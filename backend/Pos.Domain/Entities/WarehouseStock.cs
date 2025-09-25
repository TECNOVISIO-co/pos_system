namespace Pos.Domain.Entities;

public class WarehouseStock
{
    protected WarehouseStock()
    {
    }

    public WarehouseStock(Guid warehouseId, Guid productId)
    {
        Id = Guid.NewGuid();
        WarehouseId = warehouseId;
        ProductId = productId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal OnHand { get; private set; }
    public decimal Reserved { get; private set; }
    public decimal Available => OnHand - Reserved;
    public DateTimeOffset UpdatedAt { get; private set; }

    public void Adjust(decimal onHandDelta, decimal reservedDelta = 0)
    {
        OnHand += onHandDelta;
        Reserved += reservedDelta;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetAbsolute(decimal onHand, decimal reserved)
    {
        OnHand = onHand;
        Reserved = reserved;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
