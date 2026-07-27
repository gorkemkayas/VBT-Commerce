using FluentAssertions;
using Inventory.Application.Commands.StockItems.CreateStockItem;
using Inventory.Domain.Enums;
using Xunit;

namespace Inventory.Application.Tests.Commands.StockItems.CreateStockItem;

public class CreateStockItemCommandValidatorTests
{
    private readonly CreateStockItemCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateStockItemCommand(Guid.NewGuid(), InventoryItemType.Product, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var command = new CreateStockItemCommand(Guid.Empty, InventoryItemType.Product, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateStockItemCommand.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var command = new CreateStockItemCommand(Guid.NewGuid(), (InventoryItemType)999, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateStockItemCommand.SellableItemType));
    }

    [Fact]
    public void Validate_WithNegativeInitialQuantity_HasError()
    {
        var command = new CreateStockItemCommand(Guid.NewGuid(), InventoryItemType.Product, -1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateStockItemCommand.InitialQuantity));
    }
}
