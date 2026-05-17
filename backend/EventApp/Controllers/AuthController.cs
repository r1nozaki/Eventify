using EventApp.DTOs.Auth;
using EventApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EventApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(request, cancellationToken);
        SetRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiresAt);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        SetRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiresAt);
        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest? request, CancellationToken cancellationToken)
    {
        var refreshToken = request?.RefreshToken;

        if (string.IsNullOrWhiteSpace(refreshToken) && Request.Cookies.TryGetValue("refreshToken", out var cookieToken))
        {
            refreshToken = cookieToken;
        }

        var response = await authService.RefreshTokenAsync(refreshToken ?? string.Empty, cancellationToken);
        SetRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiresAt);
        return Ok(response);
    }

    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest? request, CancellationToken cancellationToken)
    {
        var refreshToken = request?.RefreshToken;

        if (string.IsNullOrWhiteSpace(refreshToken) && Request.Cookies.TryGetValue("refreshToken", out var cookieToken))
        {
            refreshToken = cookieToken;
        }

        await authService.RevokeRefreshTokenAsync(refreshToken ?? string.Empty, cancellationToken);
        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt,
            IsEssential = true
        });
    }
}
