using Pos.Domain.Entities;

namespace Pos.Infrastructure.Repositories;

public interface ITaxRepository
{
    Task<IReadOnlyList<Tax>> GetAllAsync(CancellationToken cancellationToken = default);
}
