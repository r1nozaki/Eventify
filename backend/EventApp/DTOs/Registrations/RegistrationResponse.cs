namespace EventApp.DTOs.Registrations;

public record RegistrationResponse(
    Guid Id,
    Guid UserId,
    string Username,
    Guid EventId,
    string EventTitle,
    string EventCategory,
    string EventFormat,
    string Status,
    DateTime CreatedAt);
