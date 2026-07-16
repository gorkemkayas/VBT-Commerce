using Customer.Application.Abstractions;
using Customer.Application.Common;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Queries.GuestCustomers.GetGuestCustomerById;

public class GetGuestCustomerByIdQueryHandler(ICustomerDbContext dbContext)
    : IRequestHandler<GetGuestCustomerByIdQuery, GuestCustomerDto>
{
    public async Task<GuestCustomerDto> Handle(GetGuestCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var guestCustomer = await dbContext.GuestCustomers
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.GuestCustomerId, cancellationToken)
            ?? throw new GuestCustomerNotFoundException(request.GuestCustomerId);

        return new GuestCustomerDto(guestCustomer.Id, guestCustomer.FirstName, guestCustomer.LastName, guestCustomer.Email, guestCustomer.PhoneNumber);
    }
}
