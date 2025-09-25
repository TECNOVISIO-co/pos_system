using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class Warehouse : AuditableEntity
{
    protected Warehouse()
    {
    }

    public Warehouse(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Location { get; private set; }
    public bool IsDefault { get; private set; }
    public bool AllowSales { get; private set; } = true;

    public void UpdateDetails(string name, string? location)
    {
        Name = name;
        Location = location;
        MarkUpdated();
    }

    public void SetFlags(bool isDefault, bool allowSales)
    {
        IsDefault = isDefault;
        AllowSales = allowSales;
        MarkUpdated();
    }
}
