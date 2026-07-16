using BuildingBlocks.Application.Messaging;
using Catalog.Application.Common;

namespace Catalog.Application.Queries.Products.GetProductBySlug;

public record GetProductBySlugQuery(string Slug) : IQuery<ProductDto>;
