using BuildingBlocks.Application.Exceptions;
using BuildingBlocks.Domain;
using Catalog.Domain.Exceptions;
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
            CategoryNotFoundException categoryNotFoundException => (StatusCodes.Status404NotFound, "Not Found", categoryNotFoundException.Message),
            ProductNotFoundException productNotFoundException => (StatusCodes.Status404NotFound, "Not Found", productNotFoundException.Message),
            ProductVariantNotFoundException variantNotFoundException => (StatusCodes.Status404NotFound, "Not Found", variantNotFoundException.Message),
            ProductAttributeNotFoundException attributeNotFoundException => (StatusCodes.Status404NotFound, "Not Found", attributeNotFoundException.Message),
            ProductVariantAttributeNotFoundException variantAttributeNotFoundException => (StatusCodes.Status404NotFound, "Not Found", variantAttributeNotFoundException.Message),
            ProductImageNotFoundException imageNotFoundException => (StatusCodes.Status404NotFound, "Not Found", imageNotFoundException.Message),
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
