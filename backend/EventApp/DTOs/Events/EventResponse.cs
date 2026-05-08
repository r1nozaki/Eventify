namespace EventApp.DTOs.Events;

public record EventResponse(Guid Id, string Title, string Description, DateTime Date, string Location, int Capacity, DateTime CreatedAt);
