namespace SampleSecurityProvider.Abstractions;

public record PaginatedResponse<T>
{
    public required IEnumerable<T> Items { get; init; }
    public required int CurrentPage { get; init; }
    public required int PageSize { get; init; }
    public required int TotalPages { get; init; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}