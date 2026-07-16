using FluentValidation;

namespace Customer.Application.Commands.GuestCustomers.CreateGuestCustomer;

public class CreateGuestCustomerCommandValidator : AbstractValidator<CreateGuestCustomerCommand>
{
    public CreateGuestCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
    }
}
