namespace Pos.Api.Contracts.Responses;

public record InvoiceCreatedResponse(Guid InvoiceId, string InvoiceNumber, decimal Total, decimal BalanceDue);
