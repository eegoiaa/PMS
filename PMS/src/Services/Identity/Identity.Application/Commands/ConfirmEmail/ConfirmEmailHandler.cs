using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.ConfirmEmail;

public static class ConfirmEmailHandler
{
    public static async Task Handle(
        ConfirmEmailCommand command,
        UserManager<AppUser> userManager
        )
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString()) ?? throw new IdentityException("User not found");
        var result = await userManager.ConfirmEmailAsync(user, command.Token);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors);
    }
}
