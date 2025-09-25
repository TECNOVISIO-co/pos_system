using System.Text.Json;
using Pos.Domain.Enums;

namespace Pos.Api.Contracts.Requests;

public record SyncPushRequest(string Entity, Guid ClientId, Guid BatchId, IReadOnlyList<SyncChangeRequest> Changes);

public record SyncChangeRequest(SyncOperationType Operation, Guid? EntityId, JsonElement? Data);
