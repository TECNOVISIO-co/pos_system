namespace Pos.Infrastructure.Repositories;

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
