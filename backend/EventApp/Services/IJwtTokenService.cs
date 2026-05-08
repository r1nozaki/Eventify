using EventApp.Models;

namespace EventApp.Services;

public interface IJwtTokenService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
