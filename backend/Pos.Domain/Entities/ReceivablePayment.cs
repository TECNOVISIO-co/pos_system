namespace Pos.Domain.Entities;

public class ReceivablePayment
{
    protected ReceivablePayment()
    {
    }

    public ReceivablePayment(Guid receivableId, Guid? paymentId, decimal amount, DateTimeOffset appliedAt, Guid? createdBy = null)
    {
        Id = Guid.NewGuid();
        ReceivableId = receivableId;
        PaymentId = paymentId;
        Amount = amount;
        AppliedAt = appliedAt;
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }

    public Guid Id { get; private set; }
    public Guid ReceivableId { get; private set; }
    public Guid? PaymentId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset AppliedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
}
