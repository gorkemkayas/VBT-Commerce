using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
{
    public void Configure(EntityTypeBuilder<StockReservation> builder)
    {
        builder.ToTable("StockReservations");
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.ReferenceId);
        builder.HasIndex(r => new { r.IsConfirmed, r.IsReleased, r.ExpiresAt });
    }
}
