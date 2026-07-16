using BuildingBlocks.Application.Security;
using Customer.Application.Abstractions;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Commands.Profile.UpdateMyCustomerProfile;

public class UpdateMyCustomerProfileCommandHandler(ICustomerDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateMyCustomerProfileCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMyCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
            ?? throw CustomerNotFoundException.ForUserId(userId);

        customer.UpdateProfile(request.PhoneNumber, request.DateOfBirth);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
