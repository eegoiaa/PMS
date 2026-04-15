namespace Identity.Application.Commands.SignUp;

public record SignUpCommand(
    string Email,
    string FullName,
    string Password,
    string ConfirmPassword
    );

