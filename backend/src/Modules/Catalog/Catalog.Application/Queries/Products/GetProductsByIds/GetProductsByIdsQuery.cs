using BuildingBlocks.Application.Messaging;
using Catalog.Application.Common;

namespace Catalog.Application.Queries.Products.GetProductsByIds;

public record GetProductsByIdsQuery(IReadOnlyCollection<Guid> ProductIds) : IQuery<IReadOnlyCollection<ProductDto>>;
