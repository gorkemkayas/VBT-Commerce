namespace Cart.Contracts;

/// <summary>
/// The Cart module's outbound write contract (architecture.md §3). Other modules (chiefly Order,
/// once checkout completes) depend on this instead of Cart's internal Application commands.
/// </summary>
public interface ICartMutator
{
    Task ClearByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task ClearByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken);
}
