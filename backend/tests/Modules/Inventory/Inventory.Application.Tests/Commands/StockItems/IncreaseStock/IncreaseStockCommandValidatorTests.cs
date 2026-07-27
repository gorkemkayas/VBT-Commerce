using FluentAssertions;
using Inventory.Application.Commands.StockItems.IncreaseStock;
using Xunit;

namespace Inventory.Application.Tests.Commands.StockItems.IncreaseStock;

public class IncreaseStockCommandValidatorTests
{
    private readonly IncreaseStockCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new IncreaseStockCommand(Guid.NewGuid(), 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyStockItemId_HasError()
    {
        var command = new IncreaseStockCommand(Guid.Empty, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(IncreaseStockCommand.StockItemId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveQuantity_HasError(int quantity)
    {
        var command = new IncreaseStockCommand(Guid.NewGuid(), quantity);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(IncreaseStockCommand.Quantity));
    }
}
