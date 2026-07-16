using Customer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Infrastructure.Persistence.Configurations;

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CustomerAddresses");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Label).HasMaxLength(50).IsRequired();
        builder.Property(a => a.RecipientName).HasMaxLength(150).IsRequired();
        builder.Property(a => a.PhoneNumber).HasMaxLength(30).IsRequired();
        builder.Property(a => a.Country).HasMaxLength(100).IsRequired();
        builder.Property(a => a.City).HasMaxLength(100).IsRequired();
        builder.Property(a => a.District).HasMaxLength(100).IsRequired();
        builder.Property(a => a.PostalCode).HasMaxLength(20).IsRequired();
        builder.Property(a => a.AddressLine1).HasMaxLength(300).IsRequired();
        builder.Property(a => a.AddressLine2).HasMaxLength(300);
        builder.HasIndex(a => a.CustomerId);
    }
}
