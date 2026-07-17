using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<CustomerOrder>
{
    public void Configure(EntityTypeBuilder<CustomerOrder> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.GuestCustomerId);
        builder.HasIndex(o => o.Status);

        builder.Property(o => o.RecipientName).HasMaxLength(200).IsRequired();
        builder.Property(o => o.PhoneNumber).HasMaxLength(30).IsRequired();
        builder.Property(o => o.Country).HasMaxLength(100).IsRequired();
        builder.Property(o => o.City).HasMaxLength(100).IsRequired();
        builder.Property(o => o.District).HasMaxLength(100).IsRequired();
        builder.Property(o => o.PostalCode).HasMaxLength(20).IsRequired();
        builder.Property(o => o.AddressLine1).HasMaxLength(300).IsRequired();
        builder.Property(o => o.AddressLine2).HasMaxLength(300);
        builder.Property(o => o.CancelledReason).HasMaxLength(500);

        builder.Property(o => o.ShippingFee).HasColumnType("decimal(18,2)");
        builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
        builder.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(o => o.TaxRate).HasColumnType("decimal(5,2)");
        builder.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)");
        builder.Property(o => o.GrandTotal).HasColumnType("decimal(18,2)");

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Coupons)
            .WithOne()
            .HasForeignKey(c => c.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
