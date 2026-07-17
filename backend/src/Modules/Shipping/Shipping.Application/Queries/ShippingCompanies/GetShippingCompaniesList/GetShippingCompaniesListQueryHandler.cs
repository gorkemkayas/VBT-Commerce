using MediatR;
using Microsoft.EntityFrameworkCore;
using Shipping.Application.Abstractions;
using Shipping.Application.Common;

namespace Shipping.Application.Queries.ShippingCompanies.GetShippingCompaniesList;

public class GetShippingCompaniesListQueryHandler(IShippingDbContext dbContext)
    : IRequestHandler<GetShippingCompaniesListQuery, PagedResult<ShippingCompanyDto>>
{
    public async Task<PagedResult<ShippingCompanyDto>> Handle(GetShippingCompaniesListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.ShippingCompanies.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ShippingCompanyDto>(
            items.Select(ShippingCompanyMapper.ToDto).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
