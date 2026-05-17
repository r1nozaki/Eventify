using EventApp.DTOs.Common;
using EventApp.DTOs.Events;
using EventApp.DTOs.Queries;
using EventApp.Exceptions;
using EventApp.Models;
using EventApp.Repositories;

namespace EventApp.Services;

public class EventService(IEventRepository eventRepository) : IEventService
{
    public async Task<PagedResponse<EventResponse>> GetAllAsync(EventQuery query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await eventRepository.GetAllAsync(query, cancellationToken);
        var counts = await eventRepository.GetApprovedRegistrationCountsAsync(
            items.Select(e => e.Id).ToList(),
            cancellationToken);
        var mapped = items.Select(e => Map(e, counts.GetValueOrDefault(e.Id, 0))).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedResponse<EventResponse>(mapped, query.PageNumber, query.PageSize, totalCount, totalPages);
    }

    public async Task<EventResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventEntity = await eventRepository.GetByIdAsync(id, cancellationToken)
                          ?? throw new AppException("Event not found.", StatusCodes.Status404NotFound);

        var counts = await eventRepository.GetApprovedRegistrationCountsAsync(
            new List<Guid> { eventEntity.Id },
            cancellationToken);

        return Map(eventEntity, counts.GetValueOrDefault(eventEntity.Id, 0));
    }

    public async Task<EventResponse> CreateAsync(EventRequest request, CancellationToken cancellationToken = default)
    {
        var category = NormalizeCategory(request.Category);
        var format = NormalizeFormat(request.Format);

        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Date = request.Date,
            Location = request.Location,
            Capacity = request.Capacity,
            CreatedAt = DateTime.UtcNow,
            Category = category,
            Format = format
        };

        await eventRepository.AddAsync(eventEntity, cancellationToken);
        return Map(eventEntity, 0);
    }

    public async Task<EventResponse> UpdateAsync(Guid id, EventRequest request, CancellationToken cancellationToken = default)
    {
        var eventEntity = await eventRepository.GetByIdAsync(id, cancellationToken)
                          ?? throw new AppException("Event not found.", StatusCodes.Status404NotFound);

        var approvedCount = await eventRepository.GetApprovedRegistrationCountsAsync(
            new List<Guid> { eventEntity.Id },
            cancellationToken);
        var currentApprovedCount = approvedCount.GetValueOrDefault(eventEntity.Id, 0);
        if (request.Capacity < currentApprovedCount)
        {
            throw new AppException(
                "Event capacity cannot be lower than approved registrations count.",
                StatusCodes.Status409Conflict);
        }

        eventEntity.Title = request.Title;
        eventEntity.Description = request.Description;
        eventEntity.Date = request.Date;
        eventEntity.Location = request.Location;
        eventEntity.Capacity = request.Capacity;
        eventEntity.Category = NormalizeCategory(request.Category);
        eventEntity.Format = NormalizeFormat(request.Format);

        await eventRepository.UpdateAsync(eventEntity, cancellationToken);

        return Map(eventEntity, currentApprovedCount);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventEntity = await eventRepository.GetByIdAsync(id, cancellationToken)
                          ?? throw new AppException("Event not found.", StatusCodes.Status404NotFound);

        await eventRepository.DeleteAsync(eventEntity, cancellationToken);
    }

    private static string NormalizeCategory(string? value)
        => string.IsNullOrWhiteSpace(value) ? "meetup" : value.Trim().ToLowerInvariant();

    private static string NormalizeFormat(string? value)
        => string.IsNullOrWhiteSpace(value) ? "offline" : value.Trim().ToLowerInvariant();

    private static EventResponse Map(Event eventEntity, int approvedRegistrationCount)
        => new(
            eventEntity.Id,
            eventEntity.Title,
            eventEntity.Description,
            eventEntity.Date,
            eventEntity.Location,
            eventEntity.Capacity,
            eventEntity.CreatedAt,
            eventEntity.Category,
            eventEntity.Format,
            approvedRegistrationCount);
}
