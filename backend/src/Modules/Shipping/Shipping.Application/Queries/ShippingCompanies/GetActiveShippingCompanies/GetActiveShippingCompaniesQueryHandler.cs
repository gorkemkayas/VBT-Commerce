using MediatR;
using Microsoft.EntityFrameworkCore;
using Shipping.Application.Abstractions;
using Shipping.Application.Common;

namespace Shipping.Application.Queries.ShippingCompanies.GetActiveShippingCompanies;

public class GetActiveShippingCompaniesQueryHandler(IShippingDbContext dbContext)
    : IRequestHandler<GetActiveShippingCompaniesQuery, IReadOnlyList<ShippingCompanyDto>>
{
    public async Task<IReadOnlyList<ShippingCompanyDto>> Handle(GetActiveShippingCompaniesQuery request, CancellationToken cancellationToken)
    {
        var shippingCompanies = await dbContext.ShippingCompanies
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return shippingCompanies.Select(ShippingCompanyMapper.ToDto).ToList();
    }
}
