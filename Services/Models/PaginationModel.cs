namespace Services.Models;

public class PaginationModel
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 15;
}

public class PaginationModel<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalCount { get; set; }
}