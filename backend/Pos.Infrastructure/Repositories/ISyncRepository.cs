using Pos.Domain.Entities;

namespace Pos.Infrastructure.Repositories;

public interface ISyncRepository
{
    Task<PagedResult<SyncLogEntry>> GetChangesAsync(string entityName, long? sinceId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddEntriesAsync(IEnumerable<SyncLogEntry> entries, CancellationToken cancellationToken = default);
}
