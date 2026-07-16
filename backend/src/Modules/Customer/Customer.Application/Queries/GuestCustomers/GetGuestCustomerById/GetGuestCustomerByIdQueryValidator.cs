using FluentValidation;

namespace Customer.Application.Queries.GuestCustomers.GetGuestCustomerById;

public class GetGuestCustomerByIdQueryValidator : AbstractValidator<GetGuestCustomerByIdQuery>
{
    public GetGuestCustomerByIdQueryValidator()
    {
        RuleFor(x => x.GuestCustomerId).NotEmpty();
    }
}
