using BuildingBlocks.Application.Exceptions;
using BuildingBlocks.Domain;
using FluentValidation;
using Identity.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                string.Join(" ", validationException.Errors.Select(e => e.ErrorMessage))),
            InvalidCredentialsException credentialsException => (StatusCodes.Status401Unauthorized, "Unauthorized", credentialsException.Message),
            InvalidRefreshTokenException refreshTokenException => (StatusCodes.Status401Unauthorized, "Unauthorized", refreshTokenException.Message),
            UnauthorizedException unauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", unauthorizedException.Message),
            ForbiddenException forbiddenException => (StatusCodes.Status403Forbidden, "Forbidden", forbiddenException.Message),
            DomainException domainException => (StatusCodes.Status400BadRequest, "Bad Request", domainException.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", "Please try again later.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        }, cancellationToken);

        return true;
    }
}
