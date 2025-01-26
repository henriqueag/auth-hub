namespace AuthHub.Application.Dtos;

public record PagedResponse<T>
{
    public required IEnumerable<T> Items { get; init; }
    public required int TotalRecords { get; init; }
}
