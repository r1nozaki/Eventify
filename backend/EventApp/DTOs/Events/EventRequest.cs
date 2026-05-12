namespace EventApp.DTOs.Events;

public record EventRequest(
    string Title,
    string Description,
    DateTime Date,
    string Location,
    int Capacity,
    string? Category = null,
    string? Format = null);
