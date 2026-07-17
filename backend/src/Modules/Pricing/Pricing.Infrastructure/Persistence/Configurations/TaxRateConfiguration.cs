using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pricing.Domain.Entities;

namespace Pricing.Infrastructure.Persistence.Configurations;

public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
{
    public void Configure(EntityTypeBuilder<TaxRate> builder)
    {
        builder.ToTable("TaxRates");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Rate).HasColumnType("decimal(5,2)");

        // Seeded once so a "no tax rate configured" state can never occur — Admin only ever updates it.
        // UpdatedAt is a fixed literal (not DateTime.UtcNow) so the seeded model is deterministic across
        // migration builds, per EF Core's PendingModelChangesWarning.
        builder.HasData(TaxRate.CreateDefault(20, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
    }
}
