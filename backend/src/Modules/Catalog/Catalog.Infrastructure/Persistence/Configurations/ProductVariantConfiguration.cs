using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Sku).HasMaxLength(100).IsRequired();
        builder.HasIndex(v => new { v.ProductId, v.Sku }).IsUnique();

        builder.HasMany(v => v.OptionValues)
            .WithOne()
            .HasForeignKey(ov => ov.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
