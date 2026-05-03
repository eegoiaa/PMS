using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignInResult = Identity.Application.Commands.SignIn.SignInResult;

namespace Identity.Application.Commands.Refresh;

public static class RefreshHandler
{
    public static async Task<SignInResult> Handle(
    RefreshCommand command,
    UserManager<AppUser> userManager,
    IJwtProvider jwtProvider)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == command.RefreshToken)
                ?? throw new IdentityException("Invalid session or refresh token.");

        if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await userManager.UpdateAsync(user);

            throw new IdentityException("Your session has expired. Please log in again.");
        }

        var roles = await userManager.GetRolesAsync(user);

        var newAccessToken = jwtProvider.GenerateAccessToken(user, roles);
        var newRefreshToken = jwtProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken.Token;
        user.RefreshTokenExpiryTime = newRefreshToken.ExpiryTime;

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            throw new IdentityException(updateResult.Errors);

        return new SignInResult(newAccessToken, newRefreshToken, user.Id);
    }
}
