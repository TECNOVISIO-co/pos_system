namespace Pos.Api.Contracts.Requests;

public record InvoiceRequest(
    string InvoiceNumber,
    Guid CustomerId,
    Guid WarehouseId,
    DateTimeOffset IssuedAt,
    DateTimeOffset? DueAt,
    string CurrencyCode,
    string? Notes,
    IReadOnlyList<InvoiceItemRequest> Items,
    IReadOnlyList<PaymentRequest>? Payments
);
