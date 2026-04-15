using Identity.Domain.Models;

namespace Identity.Application.Commands.SignIn;

public record SignInResult(
    string AccessToken,
    RefreshToken RefreshToken
    );

