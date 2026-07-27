using FluentAssertions;
using Inventory.Application.Commands.Reservations.ReserveStock;
using Inventory.Domain.Enums;
using Xunit;

namespace Inventory.Application.Tests.Commands.Reservations.ReserveStock;

public class ReserveStockCommandValidatorTests
{
    private readonly ReserveStockCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new ReserveStockCommand(Guid.NewGuid(), [new ReserveStockLineItem(Guid.NewGuid(), InventoryItemType.Product, 1)]);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyReferenceId_HasError()
    {
        var command = new ReserveStockCommand(Guid.Empty, [new ReserveStockLineItem(Guid.NewGuid(), InventoryItemType.Product, 1)]);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReserveStockCommand.ReferenceId));
    }

    [Fact]
    public void Validate_WithNoItems_HasError()
    {
        var command = new ReserveStockCommand(Guid.NewGuid(), []);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReserveStockCommand.Items));
    }

    [Fact]
    public void Validate_WithEmptySellableItemIdInLineItem_HasError()
    {
        var command = new ReserveStockCommand(Guid.NewGuid(), [new ReserveStockLineItem(Guid.Empty, InventoryItemType.Product, 1)]);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].SellableItemId");
    }

    [Fact]
    public void Validate_WithInvalidSellableItemTypeInLineItem_HasError()
    {
        var command = new ReserveStockCommand(Guid.NewGuid(), [new ReserveStockLineItem(Guid.NewGuid(), (InventoryItemType)999, 1)]);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].SellableItemType");
    }

    [Fact]
    public void Validate_WithZeroQuantityInLineItem_HasError()
    {
        var command = new ReserveStockCommand(Guid.NewGuid(), [new ReserveStockLineItem(Guid.NewGuid(), InventoryItemType.Product, 0)]);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].Quantity");
    }
}
