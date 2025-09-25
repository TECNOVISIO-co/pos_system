using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Contracts.Responses;
using Pos.Api.Mapping;
using Pos.Infrastructure.Repositories;

namespace Pos.Api.Controllers;

[ApiController]
[Route("api/v1/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers([FromQuery] string? query, [FromQuery(Name = "updated_since")] DateTimeOffset? updatedSince, [FromQuery] int page = 1, [FromQuery] int size = 25, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        size = Math.Clamp(size, 1, 200);
        var result = await _customerRepository.SearchAsync(query, updatedSince, page, size, cancellationToken);
        var response = new PagedResponse<CustomerResponse>(result.Items.Select(c => c.ToResponse()).ToList(), result.Total, page, size);
        return Ok(response);
    }
}
