using EventApp.DTOs.Queries;
using FluentValidation;

namespace EventApp.Validators.Events;

public class EventQueryValidator : AbstractValidator<EventQuery>
{
    private static readonly string[] SortByValues =
    [
        "date", "title", "createdat", "capacity", "popularity", "registrations", "nearest"
    ];

    public EventQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrWhiteSpace(x) || SortByValues.Contains(x.ToLowerInvariant()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", SortByValues)}.");

        RuleFor(x => x.SortOrder)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.Equals("asc", StringComparison.OrdinalIgnoreCase) || x.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortOrder must be asc or desc.");

        RuleFor(x => x.Status)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "upcoming", "ongoing", "finished" }.Contains(x.ToLowerInvariant()))
            .WithMessage("Status must be upcoming, ongoing, or finished.");

        RuleFor(x => x.Availability)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "available", "full", "spots", "spots_gt_0" }.Contains(x.ToLowerInvariant()))
            .WithMessage("Availability must be available, full, or spots.");

        RuleFor(x => x.Format)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "online", "offline", "hybrid" }.Contains(x.ToLowerInvariant()))
            .WithMessage("Format must be online, offline, or hybrid.");

        RuleFor(x => x.Category)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "conference", "meetup", "workshop", "webinar", "hackathon" }.Contains(x.ToLowerInvariant()))
            .WithMessage("Category must be conference, meetup, workshop, webinar, or hackathon.");

        RuleFor(x => x.DatePreset)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "today", "tomorrow", "week", "month" }.Contains(x.ToLowerInvariant()))
            .WithMessage("DatePreset must be today, tomorrow, week, or month.");
    }
}
