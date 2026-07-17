using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pricing.Application.Abstractions;
using Pricing.Domain.Entities;

namespace Pricing.Infrastructure.Persistence;

public class PricingDbContext(DbContextOptions<PricingDbContext> options) : DbContext(options), IPricingDbContext
{
    public DbSet<Price> Prices => Set<Price>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CouponUsage> CouponUsages => Set<CouponUsage>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pricing_schema");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PricingDbContext).Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => Database.BeginTransactionAsync(cancellationToken);
}
