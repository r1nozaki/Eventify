using System.Security.Cryptography;
using System.Text;
using EventApp.DTOs.Auth;
using EventApp.Exceptions;
using EventApp.Models;
using EventApp.Repositories;

namespace EventApp.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new AppException("Email already in use.", StatusCodes.Status409Conflict);
        }

        if (await userRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
        {
            throw new AppException("Username already in use.", StatusCodes.Status409Conflict);
        }

        var role = request.Email.EndsWith("@admin.eventify", StringComparison.OrdinalIgnoreCase)
            ? Models.Enums.UserRole.Admin
            : Models.Enums.UserRole.User;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = passwordHasher.HashPassword(request.Password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user, cancellationToken);
        return await GenerateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), cancellationToken)
                   ?? throw new AppException("Invalid email or password.", StatusCodes.Status401Unauthorized);

        var isValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isValid)
        {
            throw new AppException("Invalid email or password.", StatusCodes.Status401Unauthorized);
        }

        return await GenerateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new AppException("Refresh token is required.", StatusCodes.Status400BadRequest);
        }

        var tokenHash = HashRefreshToken(refreshToken);
        var tokenEntity = await refreshTokenRepository.GetActiveByHashAsync(tokenHash, cancellationToken)
                          ?? throw new AppException("Invalid refresh token.", StatusCodes.Status401Unauthorized);

        await refreshTokenRepository.RevokeByHashAsync(tokenHash, cancellationToken);

        var user = tokenEntity.User ?? throw new AppException("Invalid refresh token user.", StatusCodes.Status401Unauthorized);
        return await GenerateAuthResponseAsync(user, cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new AppException("Refresh token is required.", StatusCodes.Status400BadRequest);
        }

        await refreshTokenRepository.RevokeByHashAsync(HashRefreshToken(refreshToken), cancellationToken);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user, CancellationToken cancellationToken)
    {
        var (token, expiresAt) = jwtTokenService.GenerateToken(user);

        var refreshTokenRaw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpiresDays = int.TryParse(configuration["Jwt:RefreshTokenExpiresDays"], out var days) ? days : 14;

        await refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = HashRefreshToken(refreshTokenRaw),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiresDays)
        }, cancellationToken);

        return new AuthResponse(token, refreshTokenRaw, expiresAt, user.Username, user.Email, user.Role.ToString());
    }

    private static string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }
}
