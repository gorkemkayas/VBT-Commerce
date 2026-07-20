using BuildingBlocks.Application.Exceptions;
using BuildingBlocks.Domain;
using Cart.Domain.Exceptions;
using Catalog.Domain.Exceptions;
using Customer.Domain.Exceptions;
using FluentValidation;
using Identity.Domain.Exceptions;
using Inventory.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Order.Domain.Exceptions;
using Payment.Domain.Exceptions;
using Pricing.Domain.Exceptions;
using Review.Domain.Exceptions;
using Shipping.Domain.Exceptions;

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
            InvalidOrExpiredResetTokenException resetTokenException => (StatusCodes.Status400BadRequest, "Bad Request", resetTokenException.Message),
            UnauthorizedException unauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", unauthorizedException.Message),
            ForbiddenException forbiddenException => (StatusCodes.Status403Forbidden, "Forbidden", forbiddenException.Message),
            CategoryNotFoundException categoryNotFoundException => (StatusCodes.Status404NotFound, "Not Found", categoryNotFoundException.Message),
            ProductNotFoundException productNotFoundException => (StatusCodes.Status404NotFound, "Not Found", productNotFoundException.Message),
            ProductVariantNotFoundException variantNotFoundException => (StatusCodes.Status404NotFound, "Not Found", variantNotFoundException.Message),
            ProductAttributeNotFoundException attributeNotFoundException => (StatusCodes.Status404NotFound, "Not Found", attributeNotFoundException.Message),
            ProductVariantAttributeNotFoundException variantAttributeNotFoundException => (StatusCodes.Status404NotFound, "Not Found", variantAttributeNotFoundException.Message),
            ProductImageNotFoundException imageNotFoundException => (StatusCodes.Status404NotFound, "Not Found", imageNotFoundException.Message),
            StockItemNotFoundException stockItemNotFoundException => (StatusCodes.Status404NotFound, "Not Found", stockItemNotFoundException.Message),
            StockReservationNotFoundException reservationNotFoundException => (StatusCodes.Status404NotFound, "Not Found", reservationNotFoundException.Message),
            SellableItemNotFoundException sellableItemNotFoundException => (StatusCodes.Status404NotFound, "Not Found", sellableItemNotFoundException.Message),
            CustomerNotFoundException customerNotFoundException => (StatusCodes.Status404NotFound, "Not Found", customerNotFoundException.Message),
            CustomerAddressNotFoundException addressNotFoundException => (StatusCodes.Status404NotFound, "Not Found", addressNotFoundException.Message),
            GuestCustomerNotFoundException guestCustomerNotFoundException => (StatusCodes.Status404NotFound, "Not Found", guestCustomerNotFoundException.Message),
            CartNotFoundException cartNotFoundException => (StatusCodes.Status404NotFound, "Not Found", cartNotFoundException.Message),
            CartItemNotFoundException cartItemNotFoundException => (StatusCodes.Status404NotFound, "Not Found", cartItemNotFoundException.Message),
            CartSellableItemNotFoundException cartSellableItemNotFoundException => (StatusCodes.Status404NotFound, "Not Found", cartSellableItemNotFoundException.Message),
            InsufficientStockException insufficientStockException => (StatusCodes.Status400BadRequest, "Bad Request", insufficientStockException.Message),
            CartInsufficientStockException cartInsufficientStockException => (StatusCodes.Status400BadRequest, "Bad Request", cartInsufficientStockException.Message),
            PriceNotFoundException priceNotFoundException => (StatusCodes.Status404NotFound, "Not Found", priceNotFoundException.Message),
            CouponNotFoundException couponNotFoundException => (StatusCodes.Status404NotFound, "Not Found", couponNotFoundException.Message),
            PricingGuestCustomerNotFoundException pricingGuestCustomerNotFoundException => (StatusCodes.Status404NotFound, "Not Found", pricingGuestCustomerNotFoundException.Message),
            PricingSellableItemNotFoundException pricingSellableItemNotFoundException => (StatusCodes.Status404NotFound, "Not Found", pricingSellableItemNotFoundException.Message),
            ShippingCompanyNotFoundException shippingCompanyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", shippingCompanyNotFoundException.Message),
            ShipmentNotFoundException shipmentNotFoundException => (StatusCodes.Status404NotFound, "Not Found", shipmentNotFoundException.Message),
            OrderNotFoundException orderNotFoundException => (StatusCodes.Status404NotFound, "Not Found", orderNotFoundException.Message),
            PaymentNotFoundException paymentNotFoundException => (StatusCodes.Status404NotFound, "Not Found", paymentNotFoundException.Message),
            PaymentDeclinedException paymentDeclinedException => (StatusCodes.Status402PaymentRequired, "Payment Required", paymentDeclinedException.Message),
            PaymentAlreadyRefundedException paymentAlreadyRefundedException => (StatusCodes.Status400BadRequest, "Bad Request", paymentAlreadyRefundedException.Message),
            PaymentGatewayException paymentGatewayException => (StatusCodes.Status502BadGateway, "Bad Gateway", paymentGatewayException.Message),
            ReviewNotFoundException reviewNotFoundException => (StatusCodes.Status404NotFound, "Not Found", reviewNotFoundException.Message),
            ReviewSellableItemNotFoundException reviewSellableItemNotFoundException => (StatusCodes.Status404NotFound, "Not Found", reviewSellableItemNotFoundException.Message),
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
