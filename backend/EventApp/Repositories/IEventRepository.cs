using EventApp.DTOs.Queries;
using EventApp.Models;

namespace EventApp.Repositories;

public interface IEventRepository
{
    Task<(List<Event> Items, int TotalCount)> GetAllAsync(EventQuery query, CancellationToken cancellationToken = default);
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Event entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Event entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Event entity, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetApprovedRegistrationCountsAsync(
        IReadOnlyList<Guid> eventIds,
        CancellationToken cancellationToken = default);
}
