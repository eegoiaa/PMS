using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public required string FullName { get; init; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
