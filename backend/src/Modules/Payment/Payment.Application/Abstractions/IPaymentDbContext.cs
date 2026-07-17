using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Payment.Domain.Entities;

namespace Payment.Application.Abstractions;

public interface IPaymentDbContext
{
    DbSet<PaymentTransaction> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
