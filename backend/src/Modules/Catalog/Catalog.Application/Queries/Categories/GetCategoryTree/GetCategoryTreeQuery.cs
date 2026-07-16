using BuildingBlocks.Application.Messaging;
using Catalog.Application.Common;

namespace Catalog.Application.Queries.Categories.GetCategoryTree;

public record GetCategoryTreeQuery(bool IncludeInactive = false) : IQuery<IReadOnlyCollection<CategoryTreeDto>>;
