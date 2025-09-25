using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Contracts.Requests;
using Pos.Api.Contracts.Responses;
using Pos.Api.Mapping;
using Pos.Api.Services;

namespace Pos.Api.Controllers;

[ApiController]
[Route("api/v1/invoices")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceApplicationService _invoiceService;

    public InvoicesController(IInvoiceApplicationService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(InvoiceCreatedResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(CreateInvoice), new { id = invoice.Id }, invoice.ToResponse());
    }
}
