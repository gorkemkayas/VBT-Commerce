using Customer.Application.Abstractions;
using Customer.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Queries.Admin.GetCustomersList;

public class GetCustomersListQueryHandler(ICustomerDbContext dbContext)
    : IRequestHandler<GetCustomersListQuery, PagedResult<CustomerListItemDto>>
{
    public async Task<PagedResult<CustomerListItemDto>> Handle(GetCustomersListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Customers.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CustomerListItemDto(c.Id, c.UserId, c.PhoneNumber, c.Addresses.Count))
            .ToListAsync(cancellationToken);

        return new PagedResult<CustomerListItemDto>(items, request.PageNumber, request.PageSize, totalCount);
    }
}
