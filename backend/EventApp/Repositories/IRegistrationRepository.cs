using EventApp.DTOs.Queries;
using EventApp.Models;

namespace EventApp.Repositories;

public interface IRegistrationRepository
{
    Task<ParticipantRegistration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ParticipantRegistration?> GetByUserAndEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default);
    Task<(List<ParticipantRegistration> Items, int TotalCount)> GetByUserIdAsync(Guid userId, PaginationQuery query, CancellationToken cancellationToken = default);
    Task<(List<ParticipantRegistration> Items, int TotalCount)> GetAllAsync(RegistrationQuery query, CancellationToken cancellationToken = default);
    Task<int> CountApprovedByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task AddAsync(ParticipantRegistration registration, CancellationToken cancellationToken = default);
    Task UpdateAsync(ParticipantRegistration registration, CancellationToken cancellationToken = default);
    Task DeleteAsync(ParticipantRegistration registration, CancellationToken cancellationToken = default);
}
