using Pos.Domain.Entities;

namespace Pos.Infrastructure.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
