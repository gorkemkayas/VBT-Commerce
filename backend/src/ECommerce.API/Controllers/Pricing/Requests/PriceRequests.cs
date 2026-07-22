using Pricing.Domain.Enums;

namespace ECommerce.API.Controllers.Pricing.Requests;

public record CreatePriceRequest(Guid SellableItemId, PriceItemType SellableItemType, decimal Amount);

public record UpdatePriceRequest(decimal Amount);
