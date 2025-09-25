namespace Pos.Api.Contracts.Responses;

public record PriceListResponse(Guid Id, string Code, string Name, string CurrencyCode, bool IsDefault);
