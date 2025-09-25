using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class Permission : AuditableEntity
{
    protected Permission()
    {
    }

    public Permission(string code, string description, string? area = null)
    {
        Code = code;
        Description = description;
        Area = area;
    }

    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Area { get; private set; }

    public void UpdateDetails(string description, string? area)
    {
        Description = description;
        Area = area;
        MarkUpdated();
    }
}
