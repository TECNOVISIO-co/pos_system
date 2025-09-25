using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Infrastructure.Repositories;

public class TaxRepository : ITaxRepository
{
    private readonly PosDbContext _context;

    public TaxRepository(PosDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Tax>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Taxes.AsNoTracking().Where(t => t.DeletedAt == null)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
}
