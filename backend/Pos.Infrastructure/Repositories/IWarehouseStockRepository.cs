using Pos.Domain.Entities;

namespace Pos.Infrastructure.Repositories;

public interface IWarehouseStockRepository
{
    Task<WarehouseStock?> GetAsync(Guid warehouseId, Guid productId, CancellationToken cancellationToken = default);
    Task UpdateAsync(WarehouseStock stock, CancellationToken cancellationToken = default);
}
