using Identity.Application.Commands.ConfirmEmail;
using Identity.Application.Commands.Refresh;
using Identity.Application.Commands.SignIn;
using Identity.Application.Commands.SignOut;
using Identity.Application.Commands.SignUp;
using Identity.Application.Queries.CheckEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wolverine;
using SignInResult = Identity.Application.Commands.SignIn.SignInResult;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMessageBus _messageBus;
    public AuthController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }



    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
    {
        await _messageBus.InvokeAsync(command);
        return Ok(new { message = "Success! Please check your email to confirm registration." });
    }



    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        await _messageBus.InvokeAsync(command);
        return Ok(new { message = "Email confirmed successfully. You can now log in." });
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] SignInCommand command)
    {
        var result = await _messageBus.InvokeAsync<SignInResult>(command);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };
        Response.Cookies.Append("access_token", result.AccessToken, cookieOptions);

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refresh_token", result.RefreshToken.Token, refreshCookieOptions);

        return Ok(new { Message = "You've signed in successfully" });
    }



    [AllowAnonymous]
    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmail([FromQuery(Name = "email")] string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Ok(new { Message = "Email parameter is missing or empty" });

        var exists = await _messageBus.InvokeAsync<bool>(new CheckEmailQuery(email));

        if (!exists)
            return Ok(new { Message = "User with this email doesn't exist" });

        return Ok();
    }



    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> LogOut()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdString, out var userId))
            await _messageBus.InvokeAsync(new SignOutCommand(userId));

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return Ok(new { Message = "You've signed out successfully" });
    }



    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        var refresh = Request.Cookies["refresh_token"];
        if (string.IsNullOrWhiteSpace(refresh))
            return Unauthorized(new { Message = "Refresh token is missing. Please log in again." });

        var result = await _messageBus.InvokeAsync<SignInResult>(new RefreshCommand(refresh));

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };
        Response.Cookies.Append("access_token", result.AccessToken, cookieOptions);

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = result.RefreshToken.ExpiryTime
        };
        Response.Cookies.Append("refresh_token", result.RefreshToken.Token, refreshCookieOptions);

        return Ok();
    }
}
