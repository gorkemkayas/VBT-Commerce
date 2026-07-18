using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Order.Domain.Entities;

namespace Order.Application.Abstractions;

public interface IOrderDbContext
{
    DbSet<CustomerOrder> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderCoupon> OrderCoupons { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
