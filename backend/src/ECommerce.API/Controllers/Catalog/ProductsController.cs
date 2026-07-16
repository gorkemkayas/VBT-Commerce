using Catalog.Application.Commands.Products.ActivateProduct;
using Catalog.Application.Commands.Products.AddProductAttribute;
using Catalog.Application.Commands.Products.AddProductImage;
using Catalog.Application.Commands.Products.AddProductVariant;
using Catalog.Application.Commands.Products.AddProductVariantAttribute;
using Catalog.Application.Commands.Products.CreateProduct;
using Catalog.Application.Commands.Products.DeleteProduct;
using Catalog.Application.Commands.Products.RemoveProductAttribute;
using Catalog.Application.Commands.Products.RemoveProductImage;
using Catalog.Application.Commands.Products.RemoveProductVariant;
using Catalog.Application.Commands.Products.RemoveProductVariantAttribute;
using Catalog.Application.Commands.Products.SetPrimaryProductImage;
using Catalog.Application.Commands.Products.UpdateProduct;
using Catalog.Application.Commands.Products.UpdateProductAttribute;
using Catalog.Application.Commands.Products.UpdateProductVariant;
using Catalog.Application.Common;
using Catalog.Application.Queries.Products.GetProductById;
using Catalog.Application.Queries.Products.GetProductBySlug;
using Catalog.Application.Queries.Products.GetProductsList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Catalog;

[ApiController]
[Route("api/products")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetList(
        [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] Guid? categoryId,
        [FromQuery] bool? isActive, [FromQuery] string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProductsListQuery(
                pageNumber == 0 ? 1 : pageNumber,
                pageSize == 0 ? 20 : pageSize,
                categoryId,
                isActive,
                searchTerm),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid productId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(productId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<ActionResult<ProductDto>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductBySlugQuery(slug), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var productId = await sender.Send(
            new CreateProductCommand(request.Name, request.Slug, request.Description, request.CategoryId, request.ProductType),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { productId }, productId);
    }

    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> Update(Guid productId, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateProductCommand(productId, request.Name, request.Slug, request.Description, request.CategoryId),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProductCommand(productId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{productId:guid}/activate")]
    public async Task<IActionResult> Activate(Guid productId, CancellationToken cancellationToken)
    {
        await sender.Send(new ActivateProductCommand(productId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{productId:guid}/attributes")]
    public async Task<ActionResult<Guid>> AddAttribute(Guid productId, AddProductAttributeRequest request, CancellationToken cancellationToken)
    {
        var attributeId = await sender.Send(
            new AddProductAttributeCommand(productId, request.Name, request.Value, request.DisplayOrder),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { productId }, attributeId);
    }

    [HttpPut("{productId:guid}/attributes/{attributeId:guid}")]
    public async Task<IActionResult> UpdateAttribute(
        Guid productId, Guid attributeId, UpdateProductAttributeRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateProductAttributeCommand(productId, attributeId, request.Name, request.Value, request.DisplayOrder),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{productId:guid}/attributes/{attributeId:guid}")]
    public async Task<IActionResult> RemoveAttribute(Guid productId, Guid attributeId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveProductAttributeCommand(productId, attributeId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{productId:guid}/variant-attributes")]
    public async Task<ActionResult<Guid>> AddVariantAttribute(
        Guid productId, AddProductVariantAttributeRequest request, CancellationToken cancellationToken)
    {
        var variantAttributeId = await sender.Send(
            new AddProductVariantAttributeCommand(productId, request.Name, request.DisplayOrder),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { productId }, variantAttributeId);
    }

    [HttpDelete("{productId:guid}/variant-attributes/{variantAttributeId:guid}")]
    public async Task<IActionResult> RemoveVariantAttribute(Guid productId, Guid variantAttributeId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveProductVariantAttributeCommand(productId, variantAttributeId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{productId:guid}/variants")]
    public async Task<ActionResult<Guid>> AddVariant(Guid productId, AddProductVariantRequest request, CancellationToken cancellationToken)
    {
        var variantId = await sender.Send(
            new AddProductVariantCommand(productId, request.Sku, request.OptionValues),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { productId }, variantId);
    }

    [HttpPut("{productId:guid}/variants/{variantId:guid}")]
    public async Task<IActionResult> UpdateVariant(
        Guid productId, Guid variantId, UpdateProductVariantRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateProductVariantCommand(productId, variantId, request.Sku, request.OptionValues, request.IsActive),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{productId:guid}/variants/{variantId:guid}")]
    public async Task<IActionResult> RemoveVariant(Guid productId, Guid variantId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveProductVariantCommand(productId, variantId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{productId:guid}/images")]
    public async Task<ActionResult<Guid>> AddImage(Guid productId, AddProductImageRequest request, CancellationToken cancellationToken)
    {
        var imageId = await sender.Send(
            new AddProductImageCommand(productId, request.Url, request.DisplayOrder, request.IsPrimary, request.ProductVariantId),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { productId }, imageId);
    }

    [HttpDelete("{productId:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> RemoveImage(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveProductImageCommand(productId, imageId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{productId:guid}/images/{imageId:guid}/set-primary")]
    public async Task<IActionResult> SetPrimaryImage(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        await sender.Send(new SetPrimaryProductImageCommand(productId, imageId), cancellationToken);
        return NoContent();
    }
}
