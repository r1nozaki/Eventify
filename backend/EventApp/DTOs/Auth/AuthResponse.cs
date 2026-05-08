namespace EventApp.DTOs.Auth;

public record AuthResponse(string Token, string RefreshToken, DateTime ExpiresAt, string Username, string Email, string Role);
