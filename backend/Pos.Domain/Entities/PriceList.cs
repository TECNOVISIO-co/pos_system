using Pos.Domain.Common;
using Pos.Domain.ValueObjects;

namespace Pos.Domain.Entities;

public class PriceList : AuditableEntity
{
    private readonly List<PriceListItem> _items = new();

    protected PriceList()
    {
    }

    public PriceList(string code, string name, string currencyCode = "COP")
    {
        Code = code;
        Name = name;
        CurrencyCode = currencyCode;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = "COP";
    public bool IsDefault { get; private set; }
    public DateOnly? ValidFrom { get; private set; }
    public DateOnly? ValidUntil { get; private set; }

    public IReadOnlyCollection<PriceListItem> Items => _items;

    public void MarkAsDefault(bool isDefault)
    {
        IsDefault = isDefault;
        MarkUpdated();
    }

    public void SetValidity(DateOnly? from, DateOnly? until)
    {
        ValidFrom = from;
        ValidUntil = until;
        MarkUpdated();
    }

    public void UpsertItem(Guid productId, Money price, DateOnly? from = null, DateOnly? until = null)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
        {
            item = new PriceListItem(Id, productId, price, CurrencyCode);
            _items.Add(item);
        }

        item.Update(price, from, until);
        MarkUpdated();
    }
}
