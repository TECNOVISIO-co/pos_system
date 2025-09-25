namespace Pos.Domain.Entities;

public class ProductTax
{
    protected ProductTax()
    {
    }

    public ProductTax(Guid productId, Guid taxId, short priority)
    {
        ProductId = productId;
        TaxId = taxId;
        Priority = priority;
    }

    public Guid ProductId { get; private set; }
    public Guid TaxId { get; private set; }
    public short Priority { get; private set; }

    public void UpdatePriority(short priority) => Priority = priority;
}
