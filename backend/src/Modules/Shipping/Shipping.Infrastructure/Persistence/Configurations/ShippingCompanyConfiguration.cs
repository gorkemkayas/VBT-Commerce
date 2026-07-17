using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipping.Domain.Entities;

namespace Shipping.Infrastructure.Persistence.Configurations;

public class ShippingCompanyConfiguration : IEntityTypeConfiguration<ShippingCompany>
{
    public void Configure(EntityTypeBuilder<ShippingCompany> builder)
    {
        builder.ToTable("ShippingCompanies");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(c => c.Fee).HasColumnType("decimal(18,2)");
    }
}
