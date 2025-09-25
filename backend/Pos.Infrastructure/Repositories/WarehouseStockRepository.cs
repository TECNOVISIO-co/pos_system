using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Infrastructure.Repositories;

public class WarehouseStockRepository : IWarehouseStockRepository
{
    private readonly PosDbContext _context;

    public WarehouseStockRepository(PosDbContext context)
    {
        _context = context;
    }

    public Task<WarehouseStock?> GetAsync(Guid warehouseId, Guid productId, CancellationToken cancellationToken = default)
        => _context.WarehouseStocks.FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId && ws.ProductId == productId, cancellationToken);

    public Task UpdateAsync(WarehouseStock stock, CancellationToken cancellationToken = default)
    {
        _context.WarehouseStocks.Update(stock);
        return Task.CompletedTask;
    }
}
