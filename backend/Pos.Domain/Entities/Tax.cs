using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class Tax : AuditableEntity
{
    protected Tax()
    {
    }

    public Tax(string code, string name, decimal rate, string scope = "sales", bool isCompound = false)
    {
        Code = code;
        Name = name;
        Rate = rate;
        Scope = scope;
        IsCompound = isCompound;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Rate { get; private set; }
    public string Scope { get; private set; } = "sales";
    public bool IsCompound { get; private set; }

    public void Update(string name, decimal rate, string scope, bool isCompound)
    {
        Name = name;
        Rate = rate;
        Scope = scope;
        IsCompound = isCompound;
        MarkUpdated();
    }
}
