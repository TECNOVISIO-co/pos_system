using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public class Invoice : AuditableEntity
{
    private readonly List<InvoiceItem> _items = new();
    private readonly List<Payment> _payments = new();

    protected Invoice()
    {
    }

    public Invoice(string invoiceNumber, Guid customerId, DateTimeOffset issuedAt, string currencyCode)
    {
        InvoiceNumber = invoiceNumber;
        CustomerId = customerId;
        IssuedAt = issuedAt;
        CurrencyCode = currencyCode;
        Status = InvoiceStatus.Draft;
    }

    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset? DueAt { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal DiscountTotal { get; private set; }
    public decimal TaxTotal { get; private set; }
    public decimal Total { get; private set; }
    public decimal TotalPaid { get; private set; }
    public string CurrencyCode { get; private set; } = "COP";
    public string? Notes { get; private set; }

    public IReadOnlyCollection<InvoiceItem> Items => _items;
    public IReadOnlyCollection<Payment> Payments => _payments;

    public decimal BalanceDue => Math.Round(Total - TotalPaid, 2, MidpointRounding.AwayFromZero);

    public void SetDueDate(DateTimeOffset? dueAt)
    {
        DueAt = dueAt;
        MarkUpdated();
    }

    public void SetWarehouse(Guid? warehouseId)
    {
        WarehouseId = warehouseId;
        MarkUpdated();
    }

    public void SetNotes(string? notes)
    {
        Notes = notes;
        MarkUpdated();
    }

    public InvoiceItem AddItem(Guid productId, string description, decimal quantity, decimal unitPrice, decimal discountRate, IEnumerable<Tax> taxes)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero");
        }

        var item = new InvoiceItem(Id, productId, description, quantity, unitPrice, discountRate);
        item.SetLineNumber(_items.Count + 1);
        item.RecalculateTotals(taxes);
        _items.Add(item);
        RecalculateTotals();
        return item;
    }

    public void UpdateItem(Guid itemId, decimal quantity, decimal unitPrice, decimal discountRate, IEnumerable<Tax> taxes)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            throw new InvalidOperationException("Invoice item not found");
        }

        item.UpdatePricing(quantity, unitPrice, discountRate);
        item.RecalculateTotals(taxes);
        RecalculateTotals();
    }

    public void RemoveItem(Guid itemId)
    {
        var existing = _items.FirstOrDefault(i => i.Id == itemId);
        if (existing is null)
        {
            return;
        }

        _items.Remove(existing);
        ReorderLines();
        RecalculateTotals();
    }

    private void ReorderLines()
    {
        var index = 1;
        foreach (var item in _items.OrderBy(i => i.LineNumber))
        {
            item.SetLineNumber(index++);
        }
    }

    public void RecalculateTotals()
    {
        Subtotal = Math.Round(_items.Sum(i => (i.Quantity * i.UnitPrice) - i.DiscountAmount), 2, MidpointRounding.AwayFromZero);
        DiscountTotal = Math.Round(_items.Sum(i => i.DiscountAmount), 2, MidpointRounding.AwayFromZero);
        TaxTotal = Math.Round(_items.Sum(i => i.TaxTotal), 2, MidpointRounding.AwayFromZero);
        Total = Math.Round(Subtotal + TaxTotal, 2, MidpointRounding.AwayFromZero);
        TotalPaid = Math.Round(_payments.Sum(p => p.Amount), 2, MidpointRounding.AwayFromZero);
        UpdateStatus();
        MarkUpdated();
    }

    public Payment RegisterPayment(Guid? customerId, PaymentMethod method, decimal amount, DateTimeOffset paidAt, string currencyCode, string? reference = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");
        }

        var payment = new Payment(Id, customerId, method, amount, paidAt, currencyCode);
        payment.SetReference(reference);
        _payments.Add(payment);
        TotalPaid = Math.Round(_payments.Sum(p => p.Amount), 2, MidpointRounding.AwayFromZero);
        UpdateStatus();
        MarkUpdated();
        return payment;
    }

    public void AttachPayment(Payment payment)
    {
        if (!_payments.Contains(payment))
        {
            _payments.Add(payment);
            TotalPaid = Math.Round(_payments.Sum(p => p.Amount), 2, MidpointRounding.AwayFromZero);
            UpdateStatus();
            MarkUpdated();
        }
    }

    public void Post()
    {
        if (!_items.Any())
        {
            throw new InvalidOperationException("Cannot post an invoice without items");
        }

        Status = InvoiceStatus.Pending;
        MarkUpdated();
    }

    public void Cancel(string? notes = null)
    {
        Status = InvoiceStatus.Cancelled;
        Notes = notes;
        MarkUpdated();
    }

    private void UpdateStatus()
    {
        if (Status == InvoiceStatus.Cancelled)
        {
            return;
        }

        var balance = BalanceDue;
        if (balance <= 0)
        {
            Status = InvoiceStatus.Paid;
        }
        else if (TotalPaid > 0)
        {
            Status = InvoiceStatus.PartiallyPaid;
        }
        else if (Status == InvoiceStatus.Draft)
        {
            Status = InvoiceStatus.Draft;
        }
        else
        {
            Status = InvoiceStatus.Pending;
        }
    }
}
