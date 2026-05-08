using EventApp.Models.Enums;

namespace EventApp.Models;

public class ParticipantRegistration
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Event? Event { get; set; }
}
