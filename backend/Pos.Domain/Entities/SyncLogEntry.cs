using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public class SyncLogEntry
{
    protected SyncLogEntry()
    {
    }

    public SyncLogEntry(string entityName, Guid? entityId, SyncOperationType operation, string origin, string payload)
    {
        EntityName = entityName;
        EntityId = entityId;
        Operation = operation;
        Origin = origin;
        Payload = payload;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public long Id { get; private set; }
    public string EntityName { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }
    public SyncOperationType Operation { get; private set; }
    public string Origin { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public bool Processed { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public void MarkProcessed()
    {
        Processed = true;
        ErrorMessage = null;
    }

    public void MarkFailed(string error)
    {
        Processed = false;
        ErrorMessage = error;
    }
}
