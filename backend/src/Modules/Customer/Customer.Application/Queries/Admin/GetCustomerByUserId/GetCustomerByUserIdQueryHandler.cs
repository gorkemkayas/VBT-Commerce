using Customer.Application.Abstractions;
using Customer.Application.Common;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Queries.Admin.GetCustomerByUserId;

public class GetCustomerByUserIdQueryHandler(ICustomerDbContext dbContext) : IRequestHandler<GetCustomerByUserIdQuery, CustomerDto>
{
    public async Task<CustomerDto> Handle(GetCustomerByUserIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw CustomerNotFoundException.ForUserId(request.UserId);

        return CustomerMapper.ToDto(customer);
    }
}
