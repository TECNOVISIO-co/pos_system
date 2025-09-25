namespace Pos.Domain.Entities;

public class InvoiceItemTax
{
    protected InvoiceItemTax()
    {
    }

    public InvoiceItemTax(Guid taxId, decimal rate, decimal amount)
    {
        TaxId = taxId;
        Rate = rate;
        Amount = amount;
    }

    public Guid TaxId { get; private set; }
    public decimal Rate { get; private set; }
    public decimal Amount { get; private set; }
}
