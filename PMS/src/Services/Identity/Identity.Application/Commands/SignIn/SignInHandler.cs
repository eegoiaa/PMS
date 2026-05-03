using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.SignIn;

public static class SignInHandler
{
    public static async Task<SignInResult> Handle(
        SignInCommand command,
        UserManager<AppUser> userManager,
        IJwtProvider jwtProvider
        )
    {
        var user = await userManager.FindByEmailAsync(command.Email)
            ?? throw new IdentityException("Either an email or passwoed is incorrect");

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid)
            throw new IdentityException("Either an email or passwoed is incorrect");

        var roles = await userManager.GetRolesAsync(user);

        var accessToken = jwtProvider.GenerateAccessToken(user, roles);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken.Token;
        user.RefreshTokenExpiryTime = refreshToken.ExpiryTime;

        await userManager.UpdateAsync(user);
        return new SignInResult(accessToken, refreshToken, user.Id);
    }
}
