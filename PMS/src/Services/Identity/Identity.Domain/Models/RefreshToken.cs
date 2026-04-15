namespace Identity.Domain.Models;

public record RefreshToken(
    string Token,
    DateTime ExpiryTime
    );

