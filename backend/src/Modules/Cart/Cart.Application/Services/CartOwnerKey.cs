namespace Cart.Application.Services;

/// <summary>
/// Identifies who a cart belongs to — exactly one of a registered user or an anonymous client id.
/// Built by each command handler from its own trusted identity source (ICurrentUserService for
/// self-service commands, the command's own AnonymousId field for anonymous commands), never from
/// a client-supplied value on the self-service side.
/// </summary>
public class CartOwnerKey
{
    public Guid? UserId { get; }
    public Guid? AnonymousId { get; }

    private CartOwnerKey(Guid? userId, Guid? anonymousId)
    {
        UserId = userId;
        AnonymousId = anonymousId;
    }

    public static CartOwnerKey ForUser(Guid userId) => new(userId, null);

    public static CartOwnerKey ForAnonymous(Guid anonymousId) => new(null, anonymousId);
}
