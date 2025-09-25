namespace Pos.Api.Contracts.Responses;

public record CustomerResponse(Guid Id, string Code, string Name, string DocumentType, string DocumentNumber, string? Email, string? Phone, decimal CreditLimit, decimal AvailableCredit, DateTimeOffset UpdatedAt);
