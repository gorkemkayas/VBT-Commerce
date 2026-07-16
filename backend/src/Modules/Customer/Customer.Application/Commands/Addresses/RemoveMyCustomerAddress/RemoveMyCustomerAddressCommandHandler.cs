using BuildingBlocks.Application.Security;
using Customer.Application.Abstractions;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Commands.Addresses.RemoveMyCustomerAddress;

public class RemoveMyCustomerAddressCommandHandler(ICustomerDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<RemoveMyCustomerAddressCommand, Unit>
{
    public async Task<Unit> Handle(RemoveMyCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var customer = await dbContext.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
            ?? throw CustomerNotFoundException.ForUserId(userId);

        var address = customer.Addresses.FirstOrDefault(a => a.Id == request.AddressId)
            ?? throw new CustomerAddressNotFoundException(request.AddressId);

        customer.RemoveAddress(request.AddressId);
        dbContext.CustomerAddresses.Remove(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
