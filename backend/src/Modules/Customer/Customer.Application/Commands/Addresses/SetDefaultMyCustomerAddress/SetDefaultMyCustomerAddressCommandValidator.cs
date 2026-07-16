using FluentValidation;

namespace Customer.Application.Commands.Addresses.SetDefaultMyCustomerAddress;

public class SetDefaultMyCustomerAddressCommandValidator : AbstractValidator<SetDefaultMyCustomerAddressCommand>
{
    public SetDefaultMyCustomerAddressCommandValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();
    }
}
