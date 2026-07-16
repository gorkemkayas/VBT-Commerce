using BuildingBlocks.Domain;

namespace Cart.Domain.Exceptions;

public class CartNotFoundException : DomainException
{
    private CartNotFoundException(string message) : base(message)
    {
    }

    public static CartNotFoundException ForCartId(Guid cartId)
        => new($"Cart '{cartId}' was not found.");

    public static CartNotFoundException ForUserId(Guid userId)
        => new($"No cart was found for user '{userId}'.");

    public static CartNotFoundException ForAnonymousId(Guid anonymousId)
        => new($"No cart was found for anonymous id '{anonymousId}'.");
}
