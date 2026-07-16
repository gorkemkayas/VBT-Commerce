using BuildingBlocks.Application.Messaging;
using Catalog.Application.Common;

namespace Catalog.Application.Queries.Products.GetProductsList;

public record GetProductsListQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? CategoryId = null,
    bool? IsActive = null,
    string? SearchTerm = null) : IQuery<PagedResult<ProductListItemDto>>;
