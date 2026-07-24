using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipping.Domain.Entities;

namespace Shipping.Infrastructure.Persistence.Configurations;

public class ShipmentStatusHistoryConfiguration : IEntityTypeConfiguration<ShipmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<ShipmentStatusHistory> builder)
    {
        builder.ToTable("ShipmentStatusHistories");
        builder.HasKey(h => h.Id);

        builder.HasIndex(h => h.ShipmentId);

        builder.Property(h => h.TrackingNumber).HasMaxLength(100);
    }
}
