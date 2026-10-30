namespace Recommenda.Application.Common;

public sealed record PagedResult<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasPrevious { get; init; }
    public bool HasNext { get; init; }
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    public static PagedResult<T> Create(IReadOnlyList<T> items, PageRequest request, int totalItems)
    {
        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        return new PagedResult<T>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPrevious = request.Page > 1,
            HasNext = request.Page < totalPages,
            Items = items
        };
    }
}
