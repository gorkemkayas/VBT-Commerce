using FluentValidation;

namespace Cart.Application.Queries.Anonymous.GetAnonymousCart;

public class GetAnonymousCartQueryValidator : AbstractValidator<GetAnonymousCartQuery>
{
    public GetAnonymousCartQueryValidator()
    {
        RuleFor(x => x.AnonymousId).NotEmpty();
    }
}
