using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly PosDbContext _context;

    public InvoiceRepository(PosDbContext context)
    {
        _context = context;
    }

    public Task<Invoice?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
        => _context.Invoices.Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
    }
}
