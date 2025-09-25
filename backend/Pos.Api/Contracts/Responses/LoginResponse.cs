namespace Pos.Api.Contracts.Responses;

public record LoginResponse(string AccessToken, string RefreshToken, string Role);
