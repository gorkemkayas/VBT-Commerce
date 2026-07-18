using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cart.Application.Commands.MergeAnonymousCart;
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
public class AuthController(ISender sender, ILogger<AuthController> logger) : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName, request.Platform),
            cancellationToken);

        await TryMergeAnonymousCartAsync(result.AccessToken, request.AnonymousId, cancellationToken);

        return Ok(BuildResponse(result, request.Platform));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password, request.Platform), cancellationToken);

        await TryMergeAnonymousCartAsync(result.AccessToken, request.AnonymousId, cancellationToken);

        return Ok(BuildResponse(result, request.Platform));
    }

    /// <summary>
    /// Orchestrates Identity + Cart at the API composition root (Program.cs already references both
    /// modules directly, so this doesn't create a module-to-module dependency — see architecture.md
    /// §3). Merging is a non-critical side effect of login/register: a failure here (e.g. the
    /// anonymous cart no longer exists) must never fail the auth response itself.
    /// </summary>
    private async Task TryMergeAnonymousCartAsync(string accessToken, Guid? anonymousId, CancellationToken cancellationToken)
    {
        if (anonymousId is null)
            return;

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");

            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return;

            await sender.Send(new MergeAnonymousCartCommand(userId, anonymousId.Value), cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Failed to merge anonymous cart '{AnonymousId}' after auth.", anonymousId);
        }
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
            // SameSite=None (not Strict) because the web frontend is expected to live on a different
            // origin/subdomain than this API — browsers require Secure=true whenever SameSite=None,
            // which Request.IsHttps already reflects correctly once behind the reverse proxy (see
            // UseForwardedHeaders in Program.cs).
            Response.Cookies.Append(RefreshTokenCookieName, result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return new AuthResponse(result.AccessToken, result.AccessTokenExpiresAt, null);
        }

        return new AuthResponse(result.AccessToken, result.AccessTokenExpiresAt, result.RefreshToken);
    }
}
