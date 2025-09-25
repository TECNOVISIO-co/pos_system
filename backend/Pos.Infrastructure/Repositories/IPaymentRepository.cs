using Pos.Domain.Entities;

namespace Pos.Infrastructure.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
}
