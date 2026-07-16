using Cart.Application.Abstractions;
using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cart.Infrastructure.Persistence;

public class CartDbContext(DbContextOptions<CartDbContext> options) : DbContext(options), ICartDbContext
{
    public DbSet<ShoppingCart> Carts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cart_schema");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => Database.BeginTransactionAsync(cancellationToken);
}
