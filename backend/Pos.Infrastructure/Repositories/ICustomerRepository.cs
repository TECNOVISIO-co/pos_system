using Pos.Domain.Entities;

namespace Pos.Infrastructure.Repositories;

public interface ICustomerRepository
{
    Task<PagedResult<Customer>> SearchAsync(string? query, DateTimeOffset? updatedSince, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
