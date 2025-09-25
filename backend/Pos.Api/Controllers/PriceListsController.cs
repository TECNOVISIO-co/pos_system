using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Contracts.Responses;
using Pos.Api.Mapping;
using Pos.Infrastructure.Repositories;

namespace Pos.Api.Controllers;

[ApiController]
[Route("api/v1/price-lists")]
[Authorize]
public class PriceListsController : ControllerBase
{
    private readonly IPriceListRepository _priceListRepository;

    public PriceListsController(IPriceListRepository priceListRepository)
    {
        _priceListRepository = priceListRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PriceListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPriceLists([FromQuery] string? name, CancellationToken cancellationToken)
    {
        var lists = await _priceListRepository.SearchAsync(name, cancellationToken);
        return Ok(lists.Select(l => l.ToResponse()));
    }
}
