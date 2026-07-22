using Catalog.Application.Commands.Categories.CreateCategory;
using Catalog.Application.Commands.Categories.DeleteCategory;
using Catalog.Application.Commands.Categories.UpdateCategory;
using Catalog.Application.Common;
using Catalog.Application.Queries.Categories.GetCategoryById;
using Catalog.Application.Queries.Categories.GetCategoryTree;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using ECommerce.API.Controllers.Catalog.Requests;

namespace ECommerce.API.Controllers.Catalog;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ISender sender) : ControllerBase
{
    [HttpGet("{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(categoryId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IReadOnlyCollection<CategoryTreeDto>>> GetTree(
        [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoryTreeQuery(includeInactive), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var categoryId = await sender.Send(
            new CreateCategoryCommand(request.Name, request.Slug, request.Description, request.ParentCategoryId, request.DisplayOrder),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { categoryId }, categoryId);
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> Update(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateCategoryCommand(categoryId, request.Name, request.Slug, request.Description, request.DisplayOrder, request.ParentCategoryId),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
        return NoContent();
    }
}
