using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public class Payment : AuditableEntity
{
    protected Payment()
    {
    }

    public Payment(Guid? invoiceId, Guid? customerId, PaymentMethod method, decimal amount, DateTimeOffset paidAt, string currencyCode = "COP")
    {
        InvoiceId = invoiceId;
        CustomerId = customerId;
        Method = method;
        Amount = amount;
        PaidAt = paidAt;
        CurrencyCode = currencyCode;
    }

    public Guid? InvoiceId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? Reference { get; private set; }
    public DateTimeOffset PaidAt { get; private set; }
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = "COP";
    public decimal ExchangeRate { get; private set; } = 1m;
    public string? Notes { get; private set; }

    public void SetReference(string? reference)
    {
        Reference = reference;
        MarkUpdated();
    }

    public void SetNotes(string? notes)
    {
        Notes = notes;
        MarkUpdated();
    }

    public void SetExchangeRate(decimal rate)
    {
        ExchangeRate = rate;
        MarkUpdated();
    }
}
