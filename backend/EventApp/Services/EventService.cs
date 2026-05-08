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
        var mapped = items.Select(Map).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedResponse<EventResponse>(mapped, query.PageNumber, query.PageSize, totalCount, totalPages);
    }

    public async Task<EventResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventEntity = await eventRepository.GetByIdAsync(id, cancellationToken)
                          ?? throw new AppException("Event not found.", StatusCodes.Status404NotFound);

        return Map(eventEntity);
    }

    public async Task<EventResponse> CreateAsync(EventRequest request, CancellationToken cancellationToken = default)
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Date = request.Date,
            Location = request.Location,
            Capacity = request.Capacity,
            CreatedAt = DateTime.UtcNow
        };

        await eventRepository.AddAsync(eventEntity, cancellationToken);
        return Map(eventEntity);
    }

    public async Task<EventResponse> UpdateAsync(Guid id, EventRequest request, CancellationToken cancellationToken = default)
    {
        var eventEntity = await eventRepository.GetByIdAsync(id, cancellationToken)
                          ?? throw new AppException("Event not found.", StatusCodes.Status404NotFound);

        eventEntity.Title = request.Title;
        eventEntity.Description = request.Description;
        eventEntity.Date = request.Date;
        eventEntity.Location = request.Location;
        eventEntity.Capacity = request.Capacity;

        await eventRepository.UpdateAsync(eventEntity, cancellationToken);
        return Map(eventEntity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventEntity = await eventRepository.GetByIdAsync(id, cancellationToken)
                          ?? throw new AppException("Event not found.", StatusCodes.Status404NotFound);

        await eventRepository.DeleteAsync(eventEntity, cancellationToken);
    }

    private static EventResponse Map(Event eventEntity)
        => new(eventEntity.Id, eventEntity.Title, eventEntity.Description, eventEntity.Date, eventEntity.Location, eventEntity.Capacity, eventEntity.CreatedAt);
}
