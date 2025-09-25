using Pos.Domain.Entities;

namespace Pos.Infrastructure.Repositories;

public interface IPriceListRepository
{
    Task<IReadOnlyList<PriceList>> SearchAsync(string? name, CancellationToken cancellationToken = default);
}
