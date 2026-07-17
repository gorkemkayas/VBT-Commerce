using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pricing.Domain.Entities;

namespace Pricing.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
        builder.Property(c => c.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(c => c.MinCartAmount).HasColumnType("decimal(18,2)");
    }
}
