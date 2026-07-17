using BuildingBlocks.Application.Messaging;

namespace Shipping.Application.Commands.Shipments.CreateShipment;

/// <summary>
/// No IRequireRole and no controller endpoint by design — the future Order module will call this
/// in-process when an order is placed (mirrors Pricing's CommitCouponUsageCommand and Inventory's
/// Reserve/Confirm/Release: infrastructure built ahead of its only caller).
/// </summary>
public record CreateShipmentCommand(Guid OrderId, Guid ShippingCompanyId) : ICommand<Guid>;
