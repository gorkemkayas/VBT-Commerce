using BuildingBlocks.Application.Security;
using Customer.Application.Abstractions;
using Customer.Application.Common;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Queries.Profile.GetMyCustomerProfile;

public class GetMyCustomerProfileQueryHandler(ICustomerDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<GetMyCustomerProfileQuery, CustomerDto>
{
    public async Task<CustomerDto> Handle(GetMyCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var customer = await dbContext.Customers
            .AsNoTracking()
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
            ?? throw CustomerNotFoundException.ForUserId(userId);

        return CustomerMapper.ToDto(customer);
    }
}
