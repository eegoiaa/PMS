using PMS.Shared.Common.Options;

namespace Identity.Domain.Settings;

public class AuthJwtOptions : JwtOptions
{
    public required int AccessTokenExpirationMinutes { get; init; }
    public required int RefreshTokenExpirationDays { get; init; }

    public required string PrivateKey { get; init; }
}
