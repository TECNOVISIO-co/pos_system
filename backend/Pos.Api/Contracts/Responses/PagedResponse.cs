namespace Pos.Api.Contracts.Responses;

public record PagedResponse<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
