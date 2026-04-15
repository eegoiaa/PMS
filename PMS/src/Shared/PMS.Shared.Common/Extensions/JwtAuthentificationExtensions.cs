using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PMS.Shared.Common.Options;
using System.Security.Cryptography;

namespace PMS.Shared.Common.Extensions;

public static class JwtAuthentificationExtensions
{
    public static IServiceCollection AddRsaJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(j => !string.IsNullOrWhiteSpace(j.Audience), "Audience is missing ")
            .Validate(j => !string.IsNullOrWhiteSpace(j.Issuer), "Issuer is missing")
            .Validate(j => !string.IsNullOrWhiteSpace(j.PublicKey), "PublicKey is missing");

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

        var rsa = RSA.Create();
        rsa.ImportFromPem(jwtOptions.PublicKey);
        var rsaKey = new RsaSecurityKey(rsa);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateLifetime = true,
                    IssuerSigningKey = rsaKey,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["access_token"];

                        if (!string.IsNullOrEmpty(accessToken))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
} 
