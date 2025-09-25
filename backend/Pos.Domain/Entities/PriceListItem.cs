using Pos.Domain.Common;
using Pos.Domain.ValueObjects;

namespace Pos.Domain.Entities;

public class PriceListItem : AuditableEntity
{
    protected PriceListItem()
    {
    }

    public PriceListItem(Guid priceListId, Guid productId, Money price, string currencyCode)
    {
        PriceListId = priceListId;
        ProductId = productId;
        SetPrice(price, currencyCode);
    }

    public Guid PriceListId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Price { get; private set; }
    public string CurrencyCode { get; private set; } = "COP";
    public DateOnly? ValidFrom { get; private set; }
    public DateOnly? ValidUntil { get; private set; }

    public void Update(Money price, DateOnly? from, DateOnly? until)
    {
        SetPrice(price, CurrencyCode);
        ValidFrom = from;
        ValidUntil = until;
        MarkUpdated();
    }

    private void SetPrice(Money money, string currency)
    {
        Price = money.Amount;
        CurrencyCode = currency;
    }
}
