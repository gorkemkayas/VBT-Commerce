using FluentValidation;

namespace Customer.Application.Commands.Profile.UpdateMyCustomerProfile;

public class UpdateMyCustomerProfileCommandValidator : AbstractValidator<UpdateMyCustomerProfileCommand>
{
    public UpdateMyCustomerProfileCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.DateOfBirth)
            .Must(d => d < DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.DateOfBirth is not null)
            .WithMessage("DateOfBirth must be in the past.");
    }
}
