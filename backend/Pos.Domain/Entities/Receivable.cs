using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class Receivable : AuditableEntity
{
    private readonly List<ReceivablePayment> _payments = new();

    protected Receivable()
    {
    }

    public Receivable(Guid customerId, string originType, Guid originId, DateTimeOffset issuedAt, decimal amount, string currencyCode)
    {
        CustomerId = customerId;
        OriginType = originType;
        OriginId = originId;
        IssuedAt = issuedAt;
        Amount = amount;
        Balance = amount;
        CurrencyCode = currencyCode;
        Status = "open";
    }

    public Guid CustomerId { get; private set; }
    public string OriginType { get; private set; } = string.Empty;
    public Guid OriginId { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset? DueAt { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Balance { get; private set; }
    public string CurrencyCode { get; private set; } = "COP";
    public string Status { get; private set; } = "open";

    public IReadOnlyCollection<ReceivablePayment> Payments => _payments;

    public void SetDueDate(DateTimeOffset? dueAt)
    {
        DueAt = dueAt;
        MarkUpdated();
    }

    public void ApplyPayment(ReceivablePayment payment)
    {
        _payments.Add(payment);
        Balance = Math.Max(0, Balance - payment.Amount);
        UpdateStatus();
        MarkUpdated();
    }

    private void UpdateStatus()
    {
        if (Balance == 0)
        {
            Status = "paid";
        }
        else if (Balance < Amount)
        {
            Status = "partially_paid";
        }
        else
        {
            Status = "open";
        }
    }
}
