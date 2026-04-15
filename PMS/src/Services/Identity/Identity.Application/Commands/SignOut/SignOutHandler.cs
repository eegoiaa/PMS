using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.SignOut;

public static class SignOutHandler
{
    public static async Task Handle(
        SignOutCommand command,
        UserManager<AppUser> userManager
        )
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await userManager.UpdateAsync(user);
    }
}
