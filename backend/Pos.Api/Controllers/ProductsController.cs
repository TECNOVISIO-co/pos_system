using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Contracts.Responses;
using Pos.Api.Mapping;
using Pos.Infrastructure.Repositories;

namespace Pos.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] string? query, [FromQuery(Name = "updated_since")] DateTimeOffset? updatedSince, [FromQuery] int page = 1, [FromQuery] int size = 25, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        size = Math.Clamp(size, 1, 200);
        var result = await _productRepository.SearchAsync(query, updatedSince, page, size, cancellationToken);
        var response = new PagedResponse<ProductResponse>(result.Items.Select(p => p.ToResponse()).ToList(), result.Total, page, size);
        return Ok(response);
    }
}
