using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class Role : AuditableEntity
{
    private readonly List<RolePermission> _permissions = new();

    protected Role()
    {
    }

    public Role(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions;

    public void UpdateDetails(string name, string? description)
    {
        Name = name;
        Description = description;
        MarkUpdated();
    }

    public void GrantPermission(Permission permission)
    {
        if (_permissions.Any(p => p.PermissionId == permission.Id))
        {
            return;
        }

        _permissions.Add(new RolePermission(Id, permission.Id));
        MarkUpdated();
    }

    public void RevokePermission(Guid permissionId)
    {
        var existing = _permissions.FirstOrDefault(p => p.PermissionId == permissionId);
        if (existing is null)
        {
            return;
        }

        _permissions.Remove(existing);
        MarkUpdated();
    }
}
