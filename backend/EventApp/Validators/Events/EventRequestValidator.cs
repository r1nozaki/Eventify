using EventApp.DTOs.Events;
using FluentValidation;

namespace EventApp.Validators.Events;

public class EventRequestValidator : AbstractValidator<EventRequest>
{
    public EventRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Date).GreaterThan(DateTime.UtcNow.AddMinutes(-1));
        RuleFor(x => x.Capacity).GreaterThan(0).LessThanOrEqualTo(100_000);
    }
}
