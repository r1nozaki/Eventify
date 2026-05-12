namespace EventApp.DTOs.Queries;

public class EventQuery : PaginationQuery
{
    
    public string? Search { get; set; }

    
    public string? Location { get; set; }

   
    public string? Status { get; set; }

   
    public string? Availability { get; set; }

    
    public string? Format { get; set; }

   
    public string? Category { get; set; }

    
    public string? DatePreset { get; set; }
}
