namespace Pos.Domain.Common;

public abstract class AuditableEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }

    public void MarkCreated(Guid? userId = null, DateTimeOffset? timestamp = null)
    {
        CreatedAt = timestamp ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
        CreatedBy = userId;
        UpdatedBy = userId;
    }

    public void MarkUpdated(Guid? userId = null, DateTimeOffset? timestamp = null)
    {
        UpdatedAt = timestamp ?? DateTimeOffset.UtcNow;
        UpdatedBy = userId;
    }

    public void SoftDelete(Guid? userId = null, DateTimeOffset? timestamp = null)
    {
        if (DeletedAt != null)
        {
            return;
        }

        DeletedAt = timestamp ?? DateTimeOffset.UtcNow;
        UpdatedBy = userId;
        UpdatedAt = DeletedAt.Value;
    }
}
