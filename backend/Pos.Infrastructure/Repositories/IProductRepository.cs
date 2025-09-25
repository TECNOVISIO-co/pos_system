using Pos.Domain.Entities;

namespace Pos.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<PagedResult<Product>> SearchAsync(string? query, DateTimeOffset? updatedSince, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
