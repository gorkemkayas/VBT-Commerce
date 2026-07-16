using BuildingBlocks.Application.Messaging;
using Catalog.Application.Common;

namespace Catalog.Application.Queries.Products.GetProductById;

public record GetProductByIdQuery(Guid ProductId) : IQuery<ProductDto>;
