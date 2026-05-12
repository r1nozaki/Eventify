namespace EventApp.Models;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public string Category { get; set; } = "meetup";

    public string Format { get; set; } = "offline";

    public ICollection<ParticipantRegistration> Registrations { get; set; } = new List<ParticipantRegistration>();
}
