using EventApp.DTOs.Auth;
using FluentValidation;

namespace EventApp.Validators.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .Must(x => x is null || !string.IsNullOrWhiteSpace(x))
            .WithMessage("RefreshToken cannot be empty when provided.");
    }
}
