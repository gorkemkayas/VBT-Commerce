using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cart.Application.Abstractions;

public interface ICartDbContext
{
    DbSet<ShoppingCart> Carts { get; }
    DbSet<CartItem> CartItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
