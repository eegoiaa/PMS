namespace Identity.Application.Commands.ConfirmEmail;

public record ConfirmEmailCommand(
    Guid UserId,
    string Token
    );
