using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pricing.Domain.Entities;

namespace Pricing.Infrastructure.Persistence.Configurations;

public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
{
    public void Configure(EntityTypeBuilder<CouponUsage> builder)
    {
        builder.ToTable("CouponUsages");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.DiscountAmount).HasColumnType("decimal(18,2)");

        builder.HasIndex(u => u.CouponId);
        builder.HasIndex(u => new { u.CouponId, u.CustomerId });
        builder.HasIndex(u => new { u.CouponId, u.GuestCustomerId });
    }
}
