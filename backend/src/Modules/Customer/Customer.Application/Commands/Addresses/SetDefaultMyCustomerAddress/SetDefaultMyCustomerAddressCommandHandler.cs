using BuildingBlocks.Application.Security;
using Customer.Application.Abstractions;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Commands.Addresses.SetDefaultMyCustomerAddress;

public class SetDefaultMyCustomerAddressCommandHandler(ICustomerDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<SetDefaultMyCustomerAddressCommand, Unit>
{
    public async Task<Unit> Handle(SetDefaultMyCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var customer = await dbContext.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
            ?? throw CustomerNotFoundException.ForUserId(userId);

        customer.SetDefaultAddress(request.AddressId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
