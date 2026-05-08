using EventApp.Data;
using EventApp.DTOs.Queries;
using EventApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApp.Repositories;

public class EventRepository(AppDbContext dbContext) : IEventRepository
{
    public async Task<(List<Event> Items, int TotalCount)> GetAllAsync(EventQuery query, CancellationToken cancellationToken = default)
    {
        var eventsQuery = dbContext.Events.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            eventsQuery = eventsQuery.Where(x =>
                x.Title.ToLower().Contains(search) ||
                x.Description.ToLower().Contains(search) ||
                x.Location.ToLower().Contains(search));
        }

        var sortBy = (query.SortBy ?? "date").ToLowerInvariant();
        var desc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        eventsQuery = sortBy switch
        {
            "title" => desc ? eventsQuery.OrderByDescending(x => x.Title) : eventsQuery.OrderBy(x => x.Title),
            "createdat" => desc ? eventsQuery.OrderByDescending(x => x.CreatedAt) : eventsQuery.OrderBy(x => x.CreatedAt),
            _ => desc ? eventsQuery.OrderByDescending(x => x.Date) : eventsQuery.OrderBy(x => x.Date)
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
