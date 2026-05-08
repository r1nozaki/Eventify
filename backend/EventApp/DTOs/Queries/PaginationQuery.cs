namespace EventApp.DTOs.Queries;

public class PaginationQuery
{
    private const int MaxPageSize = 100;

    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : Math.Max(value, 1);
    }

    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}
