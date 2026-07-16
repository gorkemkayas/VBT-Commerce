using FluentValidation;

namespace Customer.Application.Commands.Profile.CreateMyCustomerProfile;

public class CreateMyCustomerProfileCommandValidator : AbstractValidator<CreateMyCustomerProfileCommand>
{
    public CreateMyCustomerProfileCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.DateOfBirth)
            .Must(d => d < DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.DateOfBirth is not null)
            .WithMessage("DateOfBirth must be in the past.");
    }
}
