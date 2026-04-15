using Identity.Domain.Entities;
using Identity.Domain.Models;

namespace Identity.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateAccessToken(AppUser appUser, IEnumerable<string> roles);
    RefreshToken GenerateRefreshToken();
}
