using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public class OrderCouponConfiguration : IEntityTypeConfiguration<OrderCoupon>
{
    public void Configure(EntityTypeBuilder<OrderCoupon> builder)
    {
        builder.ToTable("OrderCoupons");
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.OrderId);
        builder.Property(c => c.Code).HasMaxLength(50).IsRequired();
        builder.Property(c => c.DiscountAmount).HasColumnType("decimal(18,2)");
    }
}
