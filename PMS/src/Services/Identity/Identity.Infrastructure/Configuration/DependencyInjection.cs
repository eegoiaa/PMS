using Identity.Application.Interfaces;
using Identity.Domain.Constants;
using Identity.Domain.Entities;
using Identity.Domain.Settings;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PMS.Shared.Common.Extensions;
using System.Reflection;
using Wolverine;

namespace Identity.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(d => !string.IsNullOrWhiteSpace(d.DefaultConnection), "Database: Connection string 'DefaultConnection' is missing or empty.")
            .ValidateOnStart();

        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName))
            .Validate(a => !string.IsNullOrWhiteSpace(a.FrontendConfirmationUrl), "Frontend confirmation URL is missing or empty in configuration.")
            .ValidateOnStart();

        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .Validate(s => !string.IsNullOrWhiteSpace(s.Host), "SMTP: Host is required.")
            .Validate(s => s.Port > 0, "SMTP: Port must be a positive number.")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Username), "SMTP: Username is required.")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Password), "SMTP: Password is required.")
            .Validate(s => !string.IsNullOrWhiteSpace(s.FromEmail), "SMTP: FromEmail is required.")
            .ValidateOnStart();

        services.AddOptions<AuthJwtOptions>()
            .Bind(configuration.GetSection(AuthJwtOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.PrivateKey), "JWT: PrivateKey is required.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.PublicKey), "JWT: PublicKey is required.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "JWT: Issuer is required.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "JWT: Audience is required.")
            .Validate(o => o.AccessTokenExpirationMinutes > 0, "JWT: AccessTokenExpiration must be positive.")
            .ValidateOnStart();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(connectionString,
                x => x.MigrationsAssembly(typeof(AuthDbContext).Assembly.GetName().Name)
            )
        );

        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = AuthConstants.RequiredLength;
            options.Password.RequireDigit = AuthConstants.RequireDigit;
            options.Password.RequireLowercase = AuthConstants.RequireLowercase;
            options.Password.RequireUppercase = AuthConstants.RequireUppercase;
            options.Password.RequireNonAlphanumeric = AuthConstants.RequireNonAlphanumeric;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });

        services.AddRsaJwtAuthentication(configuration);

        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }

    public static IHostBuilder AddMessaging(this IHostBuilder host, IConfiguration configuration, Assembly assembly)
    {
        return host.UseWolverine(options =>
        {
            options.Discovery.IncludeAssembly(assembly);
        });
    }
}
