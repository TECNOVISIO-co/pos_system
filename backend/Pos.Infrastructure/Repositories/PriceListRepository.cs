using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Infrastructure.Repositories;

public class PriceListRepository : IPriceListRepository
{
    private readonly PosDbContext _context;

    public PriceListRepository(PosDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PriceList>> SearchAsync(string? name, CancellationToken cancellationToken = default)
    {
        var lists = _context.PriceLists.AsNoTracking().Where(pl => pl.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalized = name.Trim().ToLower();
            lists = lists.Where(pl => EF.Functions.ILike(pl.Name, $"%{normalized}%") || EF.Functions.ILike(pl.Code, $"%{normalized}%"));
        }

        return await lists.OrderBy(pl => pl.Name).Take(50).ToListAsync(cancellationToken);
    }
}
