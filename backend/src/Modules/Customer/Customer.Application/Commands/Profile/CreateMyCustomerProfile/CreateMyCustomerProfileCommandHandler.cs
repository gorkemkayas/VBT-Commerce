using BuildingBlocks.Application.Security;
using Customer.Application.Abstractions;
using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Commands.Profile.CreateMyCustomerProfile;

public class CreateMyCustomerProfileCommandHandler(ICustomerDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<CreateMyCustomerProfileCommand, Guid>
{
    public async Task<Guid> Handle(CreateMyCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var alreadyExists = await dbContext.Customers.AnyAsync(c => c.UserId == userId, cancellationToken);
        if (alreadyExists)
            throw new CustomerProfileAlreadyExistsException(userId);

        var customer = CustomerProfile.Create(userId, request.PhoneNumber, request.DateOfBirth);

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
