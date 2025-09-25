using Pos.Domain.Entities;

namespace Pos.Api.Auth;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
