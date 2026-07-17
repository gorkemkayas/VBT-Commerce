namespace Cart.Contracts;

public record CartSummaryDto(Guid Id, IReadOnlyCollection<CartItemSummaryDto> Items);
