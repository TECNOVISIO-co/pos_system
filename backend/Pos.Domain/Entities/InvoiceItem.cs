using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class InvoiceItem : AuditableEntity
{
    private readonly List<InvoiceItemTax> _taxes = new();

    protected InvoiceItem()
    {
    }

    public InvoiceItem(Guid invoiceId, Guid productId, string description, decimal quantity, decimal unitPrice, decimal discountRate)
    {
        InvoiceId = invoiceId;
        ProductId = productId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountRate = discountRate;
    }

    public Guid InvoiceId { get; private set; }
    public Guid ProductId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountRate { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxTotal { get; private set; }
    public decimal LineTotal { get; private set; }
    public int LineNumber { get; private set; }

    public IReadOnlyCollection<InvoiceItemTax> Taxes => _taxes;

    public void SetLineNumber(int lineNumber)
    {
        LineNumber = lineNumber;
        MarkUpdated();
    }

    public void UpdatePricing(decimal quantity, decimal unitPrice, decimal discountRate)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountRate = discountRate;
        MarkUpdated();
    }

    public void RecalculateTotals(IEnumerable<Tax> taxes)
    {
        var gross = Quantity * UnitPrice;
        DiscountAmount = Math.Round(gross * (DiscountRate / 100m), 2, MidpointRounding.AwayFromZero);
        var taxableBase = gross - DiscountAmount;

        _taxes.Clear();
        decimal totalTax = 0m;
        foreach (var tax in taxes)
        {
            var amount = Math.Round(taxableBase * (tax.Rate / 100m), 2, MidpointRounding.AwayFromZero);
            _taxes.Add(new InvoiceItemTax(tax.Id, tax.Rate, amount));
            totalTax += amount;
        }

        TaxTotal = totalTax;
        LineTotal = taxableBase + totalTax;
        MarkUpdated();
    }
}
