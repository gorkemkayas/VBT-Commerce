using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipping.Domain.Entities;

namespace Shipping.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.OrderId).IsUnique();
        builder.HasIndex(s => s.ShippingCompanyId);

        builder.Property(s => s.TrackingNumber).HasMaxLength(100);
    }
}
