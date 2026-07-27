using FluentAssertions;
using Inventory.Application.Commands.StockItems.DecreaseStock;
using Xunit;

namespace Inventory.Application.Tests.Commands.StockItems.DecreaseStock;

public class DecreaseStockCommandValidatorTests
{
    private readonly DecreaseStockCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new DecreaseStockCommand(Guid.NewGuid(), 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyStockItemId_HasError()
    {
        var command = new DecreaseStockCommand(Guid.Empty, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DecreaseStockCommand.StockItemId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveQuantity_HasError(int quantity)
    {
        var command = new DecreaseStockCommand(Guid.NewGuid(), quantity);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DecreaseStockCommand.Quantity));
    }
}
