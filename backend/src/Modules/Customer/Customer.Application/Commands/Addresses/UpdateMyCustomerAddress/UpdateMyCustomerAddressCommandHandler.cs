using BuildingBlocks.Application.Security;
using Customer.Application.Abstractions;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Commands.Addresses.UpdateMyCustomerAddress;

public class UpdateMyCustomerAddressCommandHandler(ICustomerDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateMyCustomerAddressCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMyCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var customer = await dbContext.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
            ?? throw CustomerNotFoundException.ForUserId(userId);

        customer.UpdateAddress(
            request.AddressId, request.Label, request.RecipientName, request.PhoneNumber, request.Country,
            request.City, request.District, request.PostalCode, request.AddressLine1, request.AddressLine2, request.IsDefault);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
