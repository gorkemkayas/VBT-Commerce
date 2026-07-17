using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Review.Domain.Entities;

namespace Review.Application.Abstractions;

public interface IReviewDbContext
{
    DbSet<ProductReview> Reviews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
