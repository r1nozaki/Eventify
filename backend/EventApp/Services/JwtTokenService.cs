using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventApp.Models;
using Microsoft.IdentityModel.Tokens;

namespace EventApp.Services;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public (string token, DateTime expiresAt) GenerateToken(User user)
    {
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");

        var expiresMinutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var minutes) ? minutes : 120;
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return (token, expiresAt);
    }
}
