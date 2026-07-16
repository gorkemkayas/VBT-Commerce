using FluentValidation;

namespace Customer.Application.Queries.Admin.GetCustomerByUserId;

public class GetCustomerByUserIdQueryValidator : AbstractValidator<GetCustomerByUserIdQuery>
{
    public GetCustomerByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
