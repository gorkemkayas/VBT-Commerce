using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url).HasMaxLength(2000).IsRequired();

        builder.HasOne<ProductVariant>()
            .WithMany()
            .HasForeignKey(i => i.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
