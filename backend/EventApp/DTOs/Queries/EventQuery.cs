using EventApp.DTOs.Queries;

namespace EventApp.DTOs.Queries;

public class EventQuery : PaginationQuery
{
    public string? Search { get; set; }
}
