using EventApp.DTOs.Common;
using EventApp.DTOs.Queries;
using EventApp.DTOs.Registrations;

namespace EventApp.Services;

public interface IRegistrationService
{
    Task<RegistrationResponse> RegisterAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default);
    Task<RegistrationStreakResponse> GetMyRegistrationStreakAsync(Guid userId, int? timezoneOffsetMinutes, CancellationToken cancellationToken = default);
    Task<PagedResponse<RegistrationResponse>> GetMyRegistrationsAsync(Guid userId, PaginationQuery query, CancellationToken cancellationToken = default);
    Task<PagedResponse<RegistrationResponse>> GetAllAsync(RegistrationQuery query, CancellationToken cancellationToken = default);
    Task<RegistrationResponse> UpdateStatusAsync(Guid registrationId, string status, CancellationToken cancellationToken = default);
}
