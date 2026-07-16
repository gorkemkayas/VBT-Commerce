using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("StockItems");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SellableItemType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.HasIndex(s => new { s.SellableItemId, s.SellableItemType }).IsUnique();

        builder.HasMany(s => s.Reservations)
            .WithOne()
            .HasForeignKey(r => r.StockItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
