using Pos.Domain.Enums;

namespace Pos.Api.Contracts.Requests;

public record CreatePaymentRequest(Guid? InvoiceId, Guid? CustomerId, PaymentMethod Method, decimal Amount, DateTimeOffset PaidAt, string CurrencyCode, decimal ExchangeRate, string? Reference, string? Notes);
