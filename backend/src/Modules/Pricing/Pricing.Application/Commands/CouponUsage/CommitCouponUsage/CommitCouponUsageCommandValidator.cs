using FluentValidation;

namespace Pricing.Application.Commands.CouponUsage.CommitCouponUsage;

public class CommitCouponUsageCommandValidator : AbstractValidator<CommitCouponUsageCommand>
{
    public CommitCouponUsageCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.CustomerId.HasValue != x.GuestCustomerId.HasValue)
            .WithMessage("Exactly one of CustomerId or GuestCustomerId must be provided.");
    }
}
