using Identity.Application.Commands.Login;
using Identity.Application.Commands.Logout;
using Identity.Application.Commands.RefreshTokens;
using Identity.Application.Commands.Register;
using Identity.Application.Common;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName, request.Platform),
            cancellationToken);

        return Ok(BuildResponse(result, request.Platform));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password, request.Platform), cancellationToken);

        return Ok(BuildResponse(result, request.Platform));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest? request, CancellationToken cancellationToken)
    {
        var (rawRefreshToken, platform) = ResolveIncomingRefreshToken(request?.RefreshToken);

        var result = await sender.Send(new RefreshTokenCommand(rawRefreshToken, platform), cancellationToken);

        return Ok(BuildResponse(result, platform));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest? request, CancellationToken cancellationToken)
    {
        var (rawRefreshToken, _) = ResolveIncomingRefreshToken(request?.RefreshToken);

        await sender.Send(new LogoutCommand(rawRefreshToken), cancellationToken);

        if (Request.Cookies.ContainsKey(RefreshTokenCookieName))
            Response.Cookies.Delete(RefreshTokenCookieName);

        return NoContent();
    }

    private (string RawRefreshToken, ClientPlatform Platform) ResolveIncomingRefreshToken(string? bodyRefreshToken)
    {
        var cookieToken = Request.Cookies[RefreshTokenCookieName];
        if (!string.IsNullOrEmpty(cookieToken))
            return (cookieToken, ClientPlatform.Web);

        if (!string.IsNullOrEmpty(bodyRefreshToken))
            return (bodyRefreshToken, ClientPlatform.Mobile);

        throw new BadHttpRequestException("Refresh token is required.");
    }

    private AuthResponse BuildResponse(AuthResult result, ClientPlatform platform)
    {
        if (platform == ClientPlatform.Web)
        {
            Response.Cookies.Append(RefreshTokenCookieName, result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return new AuthResponse(result.AccessToken, result.AccessTokenExpiresAt, null);
        }

        return new AuthResponse(result.AccessToken, result.AccessTokenExpiresAt, result.RefreshToken);
    }
}
