namespace Pos.Api.Contracts.Responses;

public record ProductResponse(Guid Id, string Sku, string Name, string? Description, decimal Price, decimal Cost, bool IsActive, DateTimeOffset UpdatedAt);
