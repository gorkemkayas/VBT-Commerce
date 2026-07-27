using Cart.Application.Queries.Anonymous.GetAnonymousCart;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Queries.Anonymous.GetAnonymousCart;

public class GetAnonymousCartQueryValidatorTests
{
    private readonly GetAnonymousCartQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetAnonymousCartQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAnonymousId_HasError()
    {
        var result = _validator.Validate(new GetAnonymousCartQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetAnonymousCartQuery.AnonymousId));
    }
}
