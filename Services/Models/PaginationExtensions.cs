namespace Services.Models;

public static class PaginationExtensions
{
    public static PaginationModel<T> ToPaginationModel<T>(
        this IEnumerable<T> items,
        int page,
        int size,
        int totalCount)
    {
        return new PaginationModel<T>
        {
            Items = items.ToList(),
            Page = page,
            Size = size,
            TotalCount = totalCount
        };
    }
}