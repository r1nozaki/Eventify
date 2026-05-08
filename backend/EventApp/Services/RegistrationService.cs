using EventApp.DTOs.Common;
using EventApp.DTOs.Queries;
using EventApp.DTOs.Registrations;
using EventApp.Exceptions;
using EventApp.Models;
using EventApp.Models.Enums;
using EventApp.Repositories;

namespace EventApp.Services;

public class RegistrationService(
    IRegistrationRepository registrationRepository,
    IEventRepository eventRepository,
    IUserRepository userRepository) : IRegistrationService
{
    public async Task<RegistrationResponse> RegisterAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
                   ?? throw new AppException("User not found.", StatusCodes.Status404NotFound);

        var eventEntity = await eventRepository.GetByIdAsync(eventId, cancellationToken)
                          ?? throw new AppException("Event not found.", StatusCodes.Status404NotFound);

        if (eventEntity.Date < DateTime.UtcNow)
        {
            throw new AppException("Cannot register for a past event.", StatusCodes.Status400BadRequest);
        }

        var existing = await registrationRepository.GetByUserAndEventAsync(userId, eventId, cancellationToken);
        if (existing is not null)
        {
            throw new AppException("You are already registered for this event.", StatusCodes.Status409Conflict);
        }

        var approvedCount = await registrationRepository.CountApprovedByEventIdAsync(eventId, cancellationToken);
        if (approvedCount >= eventEntity.Capacity)
        {
            throw new AppException("Event capacity reached.", StatusCodes.Status409Conflict);
        }

        var registration = new ParticipantRegistration
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = eventId,
            Status = RegistrationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            User = user,
            Event = eventEntity
        };

        await registrationRepository.AddAsync(registration, cancellationToken);
        return Map(registration);
    }

    public async Task CancelAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var registration = await registrationRepository.GetByUserAndEventAsync(userId, eventId, cancellationToken)
                           ?? throw new AppException("Registration not found.", StatusCodes.Status404NotFound);

        await registrationRepository.DeleteAsync(registration, cancellationToken);
    }

    public async Task<PagedResponse<RegistrationResponse>> GetMyRegistrationsAsync(Guid userId, PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await registrationRepository.GetByUserIdAsync(userId, query, cancellationToken);
        var mapped = items.Select(Map).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedResponse<RegistrationResponse>(mapped, query.PageNumber, query.PageSize, totalCount, totalPages);
    }

    public async Task<PagedResponse<RegistrationResponse>> GetAllAsync(RegistrationQuery query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await registrationRepository.GetAllAsync(query, cancellationToken);
        var mapped = items.Select(Map).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedResponse<RegistrationResponse>(mapped, query.PageNumber, query.PageSize, totalCount, totalPages);
    }

    public async Task<RegistrationResponse> UpdateStatusAsync(Guid registrationId, string status, CancellationToken cancellationToken = default)
    {
        var registration = await registrationRepository.GetByIdAsync(registrationId, cancellationToken)
                           ?? throw new AppException("Registration not found.", StatusCodes.Status404NotFound);

        if (!Enum.TryParse<RegistrationStatus>(status, true, out var parsedStatus))
        {
            throw new AppException("Invalid registration status.", StatusCodes.Status400BadRequest);
        }

        registration.Status = parsedStatus;
        await registrationRepository.UpdateAsync(registration, cancellationToken);

        return Map(registration);
    }

    private static RegistrationResponse Map(ParticipantRegistration registration)
        => new(
            registration.Id,
            registration.UserId,
            registration.User?.Username ?? string.Empty,
            registration.EventId,
            registration.Event?.Title ?? string.Empty,
            registration.Status.ToString(),
            registration.CreatedAt);
}
