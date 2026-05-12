using EventApp.DTOs.Events;
using FluentValidation;

namespace EventApp.Validators.Events;

public class EventRequestValidator : AbstractValidator<EventRequest>
{
    public EventRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(8000);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Date).GreaterThan(DateTime.UtcNow.AddMinutes(-30));
        RuleFor(x => x.Capacity).GreaterThan(0).LessThanOrEqualTo(100_000);

        RuleFor(x => x.Category)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "conference", "meetup", "workshop", "webinar", "hackathon" }.Contains(x!.ToLowerInvariant()))
            .WithMessage("Invalid category.");

        RuleFor(x => x.Format)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "online", "offline", "hybrid" }.Contains(x!.ToLowerInvariant()))
            .WithMessage("Invalid format.");
    }
}
