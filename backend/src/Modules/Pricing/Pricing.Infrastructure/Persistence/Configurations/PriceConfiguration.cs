using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pricing.Domain.Entities;

namespace Pricing.Infrastructure.Persistence.Configurations;

public class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        builder.ToTable("Prices");
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => new { p.SellableItemId, p.SellableItemType }).IsUnique();

        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
    }
}
