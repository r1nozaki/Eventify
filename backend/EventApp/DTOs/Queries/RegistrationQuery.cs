using EventApp.Models.Enums;

namespace EventApp.DTOs.Queries;

public class RegistrationQuery : PaginationQuery
{
    public Guid? EventId { get; set; }
    public RegistrationStatus? Status { get; set; }
}
