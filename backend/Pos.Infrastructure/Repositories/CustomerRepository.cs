using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly PosDbContext _context;

    public CustomerRepository(PosDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Customer>> SearchAsync(string? query, DateTimeOffset? updatedSince, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var customers = _context.Customers.AsNoTracking().Where(c => c.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim().ToLower();
            customers = customers.Where(c => EF.Functions.ILike(c.Code, $"%{normalized}%") || EF.Functions.ILike(c.Name, $"%{normalized}%"));
        }

        if (updatedSince.HasValue)
        {
            customers = customers.Where(c => c.UpdatedAt >= updatedSince.Value);
        }

        var total = await customers.CountAsync(cancellationToken);
        var skip = (page - 1) * pageSize;
        var items = await customers
            .OrderByDescending(c => c.UpdatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>(items, total, page, pageSize);
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
