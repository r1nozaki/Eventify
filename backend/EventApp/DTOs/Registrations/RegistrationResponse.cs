namespace EventApp.DTOs.Registrations;

public record RegistrationResponse(
    Guid Id,
    Guid UserId,
    string Username,
    Guid EventId,
    string EventTitle,
    string Status,
    DateTime CreatedAt);
