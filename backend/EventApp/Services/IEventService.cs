using EventApp.DTOs.Common;
using EventApp.DTOs.Events;
using EventApp.DTOs.Queries;

namespace EventApp.Services;

public interface IEventService
{
    Task<PagedResponse<EventResponse>> GetAllAsync(EventQuery query, CancellationToken cancellationToken = default);
    Task<EventResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EventResponse> CreateAsync(EventRequest request, CancellationToken cancellationToken = default);
    Task<EventResponse> UpdateAsync(Guid id, EventRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
