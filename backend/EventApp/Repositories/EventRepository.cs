using EventApp.Data;
using EventApp.DTOs.Queries;
using EventApp.Models;
using EventApp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventApp.Repositories;

public class EventRepository(AppDbContext dbContext) : IEventRepository
{
    public async Task<Dictionary<Guid, int>> GetApprovedRegistrationCountsAsync(
        IReadOnlyList<Guid> eventIds,
        CancellationToken cancellationToken = default)
    {
        if (eventIds.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        return await dbContext.ParticipantRegistrations
            .AsNoTracking()
            .Where(r => eventIds.Contains(r.EventId) && r.Status == RegistrationStatus.Approved)
            .GroupBy(r => r.EventId)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<(List<Event> Items, int TotalCount)> GetAllAsync(EventQuery query, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var eventsQuery = dbContext.Events.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            eventsQuery = eventsQuery.Where(x =>
                x.Title.ToLower().Contains(search) ||
                x.Description.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Location))
        {
            var loc = query.Location.Trim().ToLower();
            eventsQuery = eventsQuery.Where(x => x.Location.ToLower().Contains(loc));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var st = query.Status.Trim().ToLowerInvariant();
            if (st == "upcoming")
            {
                eventsQuery = eventsQuery.Where(e => e.Date > now);
            }
            else if (st == "finished")
            {
                eventsQuery = eventsQuery.Where(e => e.Date < now);
            }
            else if (st == "ongoing")
            {
                var windowStart = now.AddHours(-10);
                eventsQuery = eventsQuery.Where(e => e.Date <= now && e.Date >= windowStart);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Availability))
        {
            var a = query.Availability.Trim().ToLowerInvariant();
            if (a is "available" or "spots" or "spots_gt_0")
            {
                eventsQuery = eventsQuery.Where(e =>
                    e.Registrations.Count(r => r.Status == RegistrationStatus.Approved) < e.Capacity);
            }
            else if (a == "full")
            {
                eventsQuery = eventsQuery.Where(e =>
                    e.Registrations.Count(r => r.Status == RegistrationStatus.Approved) >= e.Capacity);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Format))
        {
            var f = query.Format.Trim().ToLowerInvariant();
            eventsQuery = eventsQuery.Where(e => e.Format.ToLower() == f);
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var c = query.Category.Trim().ToLowerInvariant();
            eventsQuery = eventsQuery.Where(e => e.Category.ToLower() == c);
        }

        if (!string.IsNullOrWhiteSpace(query.DatePreset))
        {
            var p = query.DatePreset.Trim().ToLowerInvariant();
            var startToday = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            switch (p)
            {
                case "today":
                {
                    var end = startToday.AddDays(1);
                    eventsQuery = eventsQuery.Where(e => e.Date >= startToday && e.Date < end);
                    break;
                }
                case "tomorrow":
                {
                    var t0 = startToday.AddDays(1);
                    eventsQuery = eventsQuery.Where(e => e.Date >= t0 && e.Date < t0.AddDays(1));
                    break;
                }
                case "week":
                {
                    eventsQuery = eventsQuery.Where(e => e.Date >= startToday && e.Date < startToday.AddDays(7));
                    break;
                }
                case "month":
                {
                    var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    eventsQuery = eventsQuery.Where(e => e.Date >= monthStart && e.Date < monthStart.AddMonths(1));
                    break;
                }
            }
        }

        var sortBy = (query.SortBy ?? "date").ToLowerInvariant();
        var desc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        eventsQuery = sortBy switch
        {
            "nearest" => eventsQuery.Where(e => e.Date >= now).OrderBy(e => e.Date),
            "popularity" or "registrations" => eventsQuery.OrderByDescending(e =>
                e.Registrations.Count(r => r.Status == RegistrationStatus.Approved)),
            "capacity" => desc
                ? eventsQuery.OrderByDescending(x => x.Capacity)
                : eventsQuery.OrderBy(x => x.Capacity),
            "title" => desc
                ? eventsQuery.OrderByDescending(x => x.Title)
                : eventsQuery.OrderBy(x => x.Title),
            "createdat" => desc
                ? eventsQuery.OrderByDescending(x => x.CreatedAt)
                : eventsQuery.OrderBy(x => x.CreatedAt),
            _ => desc
                ? eventsQuery.OrderByDescending(x => x.Date)
                : eventsQuery.OrderBy(x => x.Date)
        };

        var totalCount = await eventsQuery.CountAsync(cancellationToken);
        var items = await eventsQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task AddAsync(Event entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Events.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Event entity, CancellationToken cancellationToken = default)
    {
        dbContext.Events.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Event entity, CancellationToken cancellationToken = default)
    {
        dbContext.Events.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
