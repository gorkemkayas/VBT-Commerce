using FluentValidation;

namespace Customer.Application.Commands.Addresses.RemoveMyCustomerAddress;

public class RemoveMyCustomerAddressCommandValidator : AbstractValidator<RemoveMyCustomerAddressCommand>
{
    public RemoveMyCustomerAddressCommandValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();
    }
}
