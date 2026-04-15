namespace PMS.Shared.Common.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";
    public required string PublicKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
}
