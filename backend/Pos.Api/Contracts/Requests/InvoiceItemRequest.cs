namespace Pos.Api.Contracts.Requests;

public record InvoiceItemRequest(Guid ProductId, decimal Quantity, decimal UnitPrice, decimal DiscountRate);
