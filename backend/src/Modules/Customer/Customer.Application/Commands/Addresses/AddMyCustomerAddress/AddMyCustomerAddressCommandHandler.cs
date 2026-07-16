using BuildingBlocks.Application.Security;
using Customer.Application.Abstractions;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Commands.Addresses.AddMyCustomerAddress;

public class AddMyCustomerAddressCommandHandler(ICustomerDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<AddMyCustomerAddressCommand, Guid>
{
    public async Task<Guid> Handle(AddMyCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var customer = await dbContext.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
            ?? throw CustomerNotFoundException.ForUserId(userId);

        var address = customer.AddAddress(
            request.Label, request.RecipientName, request.PhoneNumber, request.Country, request.City,
            request.District, request.PostalCode, request.AddressLine1, request.AddressLine2, request.IsDefault);

        dbContext.CustomerAddresses.Add(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return address.Id;
    }
}
