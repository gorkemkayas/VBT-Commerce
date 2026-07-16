using BuildingBlocks.Application.Messaging;
using Cart.Application.Common;

namespace Cart.Application.Queries.Anonymous.GetAnonymousCart;

public record GetAnonymousCartQuery(Guid AnonymousId) : IQuery<CartDto>;
