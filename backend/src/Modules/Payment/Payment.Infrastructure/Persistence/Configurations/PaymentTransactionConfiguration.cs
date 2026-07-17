using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Persistence.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.OrderId).IsUnique();

        builder.Property(p => p.ProviderPaymentId).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        builder.Property(p => p.CardAssociation).HasMaxLength(50);
        builder.Property(p => p.CardFamily).HasMaxLength(50);
        builder.Property(p => p.CardLastFourDigits).HasMaxLength(4);
    }
}
