using BuildingBlocks.Application.Messaging;
using Catalog.Application.Common;

namespace Catalog.Application.Queries.Categories.GetCategoryById;

public record GetCategoryByIdQuery(Guid CategoryId) : IQuery<CategoryDto>;
