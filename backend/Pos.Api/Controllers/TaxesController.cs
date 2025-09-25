using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Contracts.Responses;
using Pos.Api.Mapping;
using Pos.Infrastructure.Repositories;

namespace Pos.Api.Controllers;

[ApiController]
[Route("api/v1/taxes")]
[Authorize]
public class TaxesController : ControllerBase
{
    private readonly ITaxRepository _taxRepository;

    public TaxesController(ITaxRepository taxRepository)
    {
        _taxRepository = taxRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaxResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaxes(CancellationToken cancellationToken)
    {
        var taxes = await _taxRepository.GetAllAsync(cancellationToken);
        return Ok(taxes.Select(t => t.ToResponse()));
    }
}
