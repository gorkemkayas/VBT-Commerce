using MediatR;
using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions;
using Payment.Application.Common;

namespace Payment.Application.Queries.GetPaymentsList;

public class GetPaymentsListQueryHandler(IPaymentDbContext dbContext)
    : IRequestHandler<GetPaymentsListQuery, PagedResult<PaymentDto>>
{
    public async Task<PagedResult<PaymentDto>> Handle(GetPaymentsListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Payments.AsNoTracking();

        if (request.Status is not null)
            query = query.Where(p => p.Status == request.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PaymentDto>(
            items.Select(PaymentMapper.ToDto).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
