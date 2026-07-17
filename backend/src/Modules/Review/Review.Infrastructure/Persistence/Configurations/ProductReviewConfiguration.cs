using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Review.Domain.Entities;

namespace Review.Infrastructure.Persistence.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.UserId, r.SellableItemId, r.SellableItemType }).IsUnique();
        builder.HasIndex(r => new { r.SellableItemId, r.SellableItemType });

        builder.Property(r => r.Comment).HasMaxLength(2000).IsRequired();
    }
}
