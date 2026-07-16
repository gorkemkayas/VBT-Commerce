using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");
        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.CartId);
        builder.HasIndex(i => new { i.CartId, i.SellableItemId, i.SellableItemType }).IsUnique();
    }
}
