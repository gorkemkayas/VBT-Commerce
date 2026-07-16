using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart.Infrastructure.Persistence.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("Carts");
        builder.HasKey(c => c.Id);

        // Filtered unique indexes: only rows with a non-null value are constrained, since UserId and
        // AnonymousId are each null for the opposite cart type (SQL Server treats every NULL as
        // distinct in a regular unique index, but the filter makes the intent explicit).
        builder.HasIndex(c => c.UserId).IsUnique().HasFilter("[UserId] IS NOT NULL");
        builder.HasIndex(c => c.AnonymousId).IsUnique().HasFilter("[AnonymousId] IS NOT NULL");

        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
