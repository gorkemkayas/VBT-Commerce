using FluentValidation;

namespace Payment.Application.Queries.GetPaymentById;

public class GetPaymentByIdQueryValidator : AbstractValidator<GetPaymentByIdQuery>
{
    public GetPaymentByIdQueryValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
    }
}
