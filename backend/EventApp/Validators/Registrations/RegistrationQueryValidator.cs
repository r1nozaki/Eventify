using EventApp.DTOs.Queries;
using FluentValidation;

namespace EventApp.Validators.Registrations;

public class RegistrationQueryValidator : AbstractValidator<RegistrationQuery>
{
    public RegistrationQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrWhiteSpace(x) || new[] { "createdat", "status" }.Contains(x.ToLowerInvariant()))
            .WithMessage("SortBy must be one of: createdAt, status.");

        RuleFor(x => x.SortOrder)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.Equals("asc", StringComparison.OrdinalIgnoreCase) || x.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortOrder must be asc or desc.");
    }
}
