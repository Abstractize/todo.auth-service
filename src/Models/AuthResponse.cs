namespace Models;

public record AuthResponse(string Token, string RefreshToken, DateTime ExpiresAt);