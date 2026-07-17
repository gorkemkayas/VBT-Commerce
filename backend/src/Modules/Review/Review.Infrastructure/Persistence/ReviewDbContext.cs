using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Review.Application.Abstractions;
using Review.Domain.Entities;

namespace Review.Infrastructure.Persistence;

public class ReviewDbContext(DbContextOptions<ReviewDbContext> options) : DbContext(options), IReviewDbContext
{
    public DbSet<ProductReview> Reviews => Set<ProductReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("review_schema");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReviewDbContext).Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => Database.BeginTransactionAsync(cancellationToken);
}
