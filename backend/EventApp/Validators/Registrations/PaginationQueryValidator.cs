using EventApp.DTOs.Queries;
using FluentValidation;

namespace EventApp.Validators.Registrations;

public class PaginationQueryValidator : AbstractValidator<PaginationQuery>
{
    public PaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
