using Identity.Application.Interfaces;
using Identity.Domain.Constants;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Identity.Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Application.Commands.SignUp;

public static class SignUpHandler
{
    public static async Task Handle(
    SignUpCommand command,
    UserManager<AppUser> userManager,
    IEmailService emailService,
    IOptions<AuthOptions> options,
    CancellationToken cancellationToken
    )
    {
        var user = new AppUser
        {
            UserName = command.Email,
            Email = command.Email,
            FullName = command.Email,
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
            throw new IdentityException(result.Errors);

        var roleResult = await userManager.AddToRoleAsync(user, RolesConstants.Developer);

        if (!roleResult.Succeeded)
            throw new IdentityException(roleResult.Errors);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var settings = options.Value;
        var confirmationLink = $"{settings.FrontendConfirmationUrl}?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        await emailService.SendConfirmationLinkAsync(user.Email, confirmationLink, cancellationToken);
    }
}
