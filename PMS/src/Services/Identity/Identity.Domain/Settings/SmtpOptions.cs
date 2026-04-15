namespace Identity.Domain.Settings;

public class SmtpOptions
{
    public const string SectionName = "SmtpSettings";
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string FromEmail { get; init; }
}
