namespace Pos.Api.Contracts.Responses;

public record TaxResponse(Guid Id, string Code, string Name, decimal Rate, string Scope, bool IsCompound);
