namespace Identity.Domain.Settings;

public class AuthOptions
{
    public const string SectionName = "AuthSettings";
    public required string FrontendConfirmationUrl { get; init; }
}
