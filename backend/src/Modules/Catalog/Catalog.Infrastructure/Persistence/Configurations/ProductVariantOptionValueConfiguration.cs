using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductVariantOptionValueConfiguration : IEntityTypeConfiguration<ProductVariantOptionValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantOptionValue> builder)
    {
        builder.ToTable("ProductVariantOptionValues");
        builder.HasKey(ov => ov.Id);

        builder.Property(ov => ov.Value).HasMaxLength(200).IsRequired();
        builder.HasIndex(ov => new { ov.ProductVariantId, ov.ProductVariantAttributeId }).IsUnique();

        builder.HasOne<ProductVariantAttribute>()
            .WithMany()
            .HasForeignKey(ov => ov.ProductVariantAttributeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
