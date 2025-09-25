using Pos.Domain.Enums;

namespace Pos.Api.Contracts.Responses;

public record SyncChangesResponse(IReadOnlyList<SyncChangeResponse> Changes, long? NextCursor, int Total);

public record SyncChangeResponse(long Id, string Entity, Guid? EntityId, SyncOperationType Operation, string Payload, DateTimeOffset CreatedAt);
