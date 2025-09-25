using Pos.Api.Contracts.Requests;
using Pos.Domain.Entities;

namespace Pos.Api.Services;

public interface IInvoiceApplicationService
{
    Task<Invoice> CreateAsync(InvoiceRequest request, CancellationToken cancellationToken = default);
}
