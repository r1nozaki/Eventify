using EventApp.DTOs.Registrations;
using FluentValidation;

namespace EventApp.Validators.Registrations;

public class RegistrationStatusUpdateRequestValidator : AbstractValidator<RegistrationStatusUpdateRequest>
{
    public RegistrationStatusUpdateRequestValidator()
    {
        RuleFor(x => x.Status).NotEmpty();
    }
}
