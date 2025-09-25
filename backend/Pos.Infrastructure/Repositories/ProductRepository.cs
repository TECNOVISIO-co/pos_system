using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly PosDbContext _context;

    public ProductRepository(PosDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Product>> SearchAsync(string? query, DateTimeOffset? updatedSince, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var products = _context.Products.AsNoTracking().Where(p => p.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim().ToLower();
            products = products.Where(p => EF.Functions.ILike(p.Sku, $"%{normalized}%") || EF.Functions.ILike(p.Name, $"%{normalized}%"));
        }

        if (updatedSince.HasValue)
        {
            products = products.Where(p => p.UpdatedAt >= updatedSince.Value);
        }

        var total = await products.CountAsync(cancellationToken);
        var skip = (page - 1) * pageSize;
        var items = await products
            .OrderByDescending(p => p.UpdatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, total, page, pageSize);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
}
