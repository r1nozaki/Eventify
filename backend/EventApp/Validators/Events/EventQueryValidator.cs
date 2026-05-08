using EventApp.DTOs.Queries;
using FluentValidation;

namespace EventApp.Validators.Events;

public class EventQueryValidator : AbstractValidator<EventQuery>
{
    public EventQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "date", "title", "createdat" }.Contains(x.ToLowerInvariant()))
            .WithMessage("SortBy must be one of: date, title, createdAt.");

        RuleFor(x => x.SortOrder)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.Equals("asc", StringComparison.OrdinalIgnoreCase) || x.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortOrder must be asc or desc.");
    }
}
