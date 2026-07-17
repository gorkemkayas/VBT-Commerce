using BuildingBlocks.Application.Messaging;
using Order.Application.Common;

namespace Order.Application.Queries.Guest.GetGuestOrderById;

public record GetGuestOrderByIdQuery(Guid GuestCustomerId, Guid OrderId) : IQuery<OrderDto>;
