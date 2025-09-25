namespace Pos.Api.Auth;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
    public int AccessTokenMinutes { get; init; } = 30;
    public int RefreshTokenMinutes { get; init; } = 43200; // 30 days
}
