using Pos.Api.Contracts.Requests;
using Pos.Api.Contracts.Responses;

namespace Pos.Api.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
