using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Infrastructure.Repositories;

public class SyncRepository : ISyncRepository
{
    private readonly PosDbContext _context;

    public SyncRepository(PosDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SyncLogEntry>> GetChangesAsync(string entityName, long? sinceId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.SyncLog.AsNoTracking().Where(s => s.EntityName == entityName);

        if (sinceId.HasValue)
        {
            query = query.Where(s => s.Id > sinceId.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var skip = (page - 1) * pageSize;
        var items = await query.OrderBy(s => s.Id).Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<SyncLogEntry>(items, total, page, pageSize);
    }

    public async Task AddEntriesAsync(IEnumerable<SyncLogEntry> entries, CancellationToken cancellationToken = default)
    {
        await _context.SyncLog.AddRangeAsync(entries, cancellationToken);
    }
}
