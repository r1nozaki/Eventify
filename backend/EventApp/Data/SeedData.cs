using EventApp.Models;

namespace EventApp.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        if (dbContext.Events.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var events = new List<Event>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ASP.NET Core Meetup",
                Description = "Community meetup for .NET developers.",
                Date = now.AddDays(10),
                Location = "Kyiv Tech Hub",
                Capacity = 120,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Frontend Next.js Workshop",
                Description = "Hands-on workshop for building modern SPAs.",
                Date = now.AddDays(20),
                Location = "Online",
                Capacity = 250,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Startup Pitch Night",
                Description = "Pitch your product and meet investors.",
                Date = now.AddDays(35),
                Location = "Eventify Center",
                Capacity = 80,
                CreatedAt = now
            }
        };

        await dbContext.Events.AddRangeAsync(events);
        await dbContext.SaveChangesAsync();
    }
}
