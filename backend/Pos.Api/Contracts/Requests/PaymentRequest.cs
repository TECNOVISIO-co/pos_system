using Pos.Domain.Enums;

namespace Pos.Api.Contracts.Requests;

public record PaymentRequest(PaymentMethod Method, decimal Amount, DateTimeOffset PaidAt, string CurrencyCode, string? Reference);
