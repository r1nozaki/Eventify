using EventApp.Data;
using EventApp.DTOs.Queries;
using EventApp.Models;
using EventApp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventApp.Repositories;

public class RegistrationRepository(AppDbContext dbContext) : IRegistrationRepository
{
    public Task<ParticipantRegistration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.ParticipantRegistrations
            .Include(r => r.User)
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<ParticipantRegistration?> GetByUserAndEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
        => dbContext.ParticipantRegistrations
            .Include(r => r.User)
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EventId == eventId, cancellationToken);

    public Task<List<DateTime>> GetCreatedAtByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => dbContext.ParticipantRegistrations
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Select(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<(List<ParticipantRegistration> Items, int TotalCount)> GetByUserIdAsync(Guid userId, PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var registrations = dbContext.ParticipantRegistrations
            .Include(r => r.Event)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await registrations.CountAsync(cancellationToken);
        var items = await registrations
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<ParticipantRegistration> Items, int TotalCount)> GetAllAsync(RegistrationQuery query, CancellationToken cancellationToken = default)
    {
        var registrations = dbContext.ParticipantRegistrations
            .Include(r => r.User)
            .Include(r => r.Event)
            .AsQueryable();

        if (query.EventId.HasValue)
        {
            registrations = registrations.Where(r => r.EventId == query.EventId.Value);
        }

        if (query.Status.HasValue)
        {
            registrations = registrations.Where(r => r.Status == query.Status.Value);
        }

        var sortBy = (query.SortBy ?? "createdat").ToLowerInvariant();
        var desc = !string.Equals(query.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);

        registrations = sortBy switch
        {
            "status" => desc ? registrations.OrderByDescending(r => r.Status) : registrations.OrderBy(r => r.Status),
            _ => desc ? registrations.OrderByDescending(r => r.CreatedAt) : registrations.OrderBy(r => r.CreatedAt)
        };

        var totalCount = await registrations.CountAsync(cancellationToken);
        var items = await registrations
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<int> CountApprovedByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        => dbContext.ParticipantRegistrations.CountAsync(r => r.EventId == eventId && r.Status == RegistrationStatus.Approved, cancellationToken);

    public async Task AddAsync(ParticipantRegistration registration, CancellationToken cancellationToken = default)
    {
        await dbContext.ParticipantRegistrations.AddAsync(registration, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ParticipantRegistration registration, CancellationToken cancellationToken = default)
    {
        dbContext.ParticipantRegistrations.Update(registration);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ParticipantRegistration registration, CancellationToken cancellationToken = default)
    {
        dbContext.ParticipantRegistrations.Remove(registration);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
