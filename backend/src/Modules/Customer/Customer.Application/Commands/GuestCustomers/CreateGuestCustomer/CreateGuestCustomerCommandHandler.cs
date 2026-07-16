using Customer.Application.Abstractions;
using Customer.Domain.Entities;
using MediatR;

namespace Customer.Application.Commands.GuestCustomers.CreateGuestCustomer;

public class CreateGuestCustomerCommandHandler(ICustomerDbContext dbContext) : IRequestHandler<CreateGuestCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateGuestCustomerCommand request, CancellationToken cancellationToken)
    {
        var guestCustomer = GuestCustomer.Create(request.FirstName, request.LastName, request.Email, request.PhoneNumber);

        dbContext.GuestCustomers.Add(guestCustomer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return guestCustomer.Id;
    }
}
