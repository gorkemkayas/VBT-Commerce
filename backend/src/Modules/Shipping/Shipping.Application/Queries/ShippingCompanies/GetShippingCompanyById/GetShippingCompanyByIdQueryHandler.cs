using MediatR;
using Shipping.Application.Abstractions;
using Shipping.Application.Common;
using Shipping.Domain.Exceptions;

namespace Shipping.Application.Queries.ShippingCompanies.GetShippingCompanyById;

public class GetShippingCompanyByIdQueryHandler(IShippingDbContext dbContext) : IRequestHandler<GetShippingCompanyByIdQuery, ShippingCompanyDto>
{
    public async Task<ShippingCompanyDto> Handle(GetShippingCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var shippingCompany = await dbContext.ShippingCompanies.FindAsync([request.ShippingCompanyId], cancellationToken)
            ?? throw new ShippingCompanyNotFoundException(request.ShippingCompanyId);

        return ShippingCompanyMapper.ToDto(shippingCompany);
    }
}
