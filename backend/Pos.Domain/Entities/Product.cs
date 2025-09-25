using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class Product : AuditableEntity
{
    private readonly List<ProductTax> _taxes = new();

    protected Product()
    {
    }

    public Product(string sku, string name, decimal price)
    {
        Sku = sku;
        Name = name;
        Price = price;
    }

    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Barcode { get; private set; }
    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public string UnitOfMeasure { get; private set; } = "und";
    public decimal Cost { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; } = true;
    public decimal MinStock { get; private set; }
    public bool AllowNegativeStock { get; private set; }
    public Guid? TaxRuleId { get; private set; }

    public IReadOnlyCollection<ProductTax> Taxes => _taxes;

    public void UpdateDetails(string name, string? description, string? barcode, string unitOfMeasure)
    {
        Name = name;
        Description = description;
        Barcode = barcode;
        UnitOfMeasure = unitOfMeasure;
        MarkUpdated();
    }

    public void UpdatePricing(decimal cost, decimal price)
    {
        Cost = cost;
        Price = price;
        MarkUpdated();
    }

    public void SetStockPolicy(decimal minStock, bool allowNegative)
    {
        MinStock = minStock;
        AllowNegativeStock = allowNegative;
        MarkUpdated();
    }

    public void SetBranding(string? brand, string? model)
    {
        Brand = brand;
        Model = model;
        MarkUpdated();
    }

    public void AssignTax(Guid taxId, short priority)
    {
        var existing = _taxes.FirstOrDefault(t => t.TaxId == taxId);
        if (existing is null)
        {
            _taxes.Add(new ProductTax(Id, taxId, priority));
        }
        else
        {
            existing.UpdatePriority(priority);
        }

        MarkUpdated();
    }

    public void RemoveTax(Guid taxId)
    {
        var existing = _taxes.FirstOrDefault(t => t.TaxId == taxId);
        if (existing is null)
        {
            return;
        }

        _taxes.Remove(existing);
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
