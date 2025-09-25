using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class User : AuditableEntity
{
    private readonly List<UserRole> _roles = new();

    protected User()
    {
    }

    public User(string username, string fullName, string passwordHash)
    {
        Username = username;
        FullName = fullName;
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public string Username { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }

    public IReadOnlyCollection<UserRole> Roles => _roles;

    public void SetEmail(string? email) => Email = email;

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
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

    public void RegisterLogin(DateTimeOffset timestamp)
    {
        LastLoginAt = timestamp;
        MarkUpdated();
    }

    public void AssignRole(Role role)
    {
        if (_roles.Any(r => r.RoleId == role.Id))
        {
            return;
        }

        _roles.Add(new UserRole(Id, role.Id));
        MarkUpdated();
    }

    public void RemoveRole(Guid roleId)
    {
        var existing = _roles.FirstOrDefault(r => r.RoleId == roleId);
        if (existing is null)
        {
            return;
        }

        _roles.Remove(existing);
        MarkUpdated();
    }
}
