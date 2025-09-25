using Pos.Api.Contracts.Requests;
using Pos.Domain.Entities;

namespace Pos.Api.Services;

public interface IPaymentApplicationService
{
    Task<Payment> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);
}
