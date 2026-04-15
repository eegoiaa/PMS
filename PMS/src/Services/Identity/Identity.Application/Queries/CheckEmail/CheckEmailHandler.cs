using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Queries.CheckEmail;

public static class CheckEmailHandler
{
    public static async Task<bool> Handle(
        CheckEmailQuery query,
        UserManager<AppUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(query.Email);
        return user != null;
    }
}
