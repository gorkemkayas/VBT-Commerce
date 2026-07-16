using Customer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Infrastructure.Persistence.Configurations;

public class GuestCustomerConfiguration : IEntityTypeConfiguration<GuestCustomer>
{
    public void Configure(EntityTypeBuilder<GuestCustomer> builder)
    {
        builder.ToTable("GuestCustomers");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(g => g.LastName).HasMaxLength(100).IsRequired();
        builder.Property(g => g.Email).HasMaxLength(256).IsRequired();
        builder.Property(g => g.PhoneNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(g => g.Email);
    }
}
