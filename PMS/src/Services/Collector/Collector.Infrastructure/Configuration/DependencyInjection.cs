using Collector.Application.Interfaces;
using Collector.Domain.Options;
using Collector.Domain.Settings;
using Collector.Infrastructure.Persistence;
using Collector.Infrastructure.Providers;
using Collector.Infrastructure.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PMS.Shared.Common.Events;
using Refit;
using System.Net.Http.Headers;
using System.Reflection;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Collector.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(d => !string.IsNullOrWhiteSpace(d.DefaultConnection), "Database: Connection string 'DefaultConnection' is missing or empty.")
            .ValidateOnStart();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CollectorDbContext>(options =>
            options.UseNpgsql(connectionString,
                x => x.MigrationsAssembly(typeof(CollectorDbContext).Assembly.GetName().Name)
            )
        );

        services.AddScoped<ICollectorDbContext>(provider => provider.GetRequiredService<CollectorDbContext>());

        services.Configure<GitHubSettings>(configuration.GetSection(GitHubSettings.SectionName));
        services.Configure<WakaTimeSettings>(configuration.GetSection(WakaTimeSettings.SectionName));

        services.AddScoped<IGitProvider, GitHubProvider>();
        services.AddScoped<IActivityCleaner, ActivityCleaner>();

        services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UsePostgreSqlStorage(options =>
                  {
                      options.UseNpgsqlConnection(connectionString);
                  }));

        services.AddHangfireServer();

        services.AddRefitClient<IWakaTimeProvider>()
            .ConfigureHttpClient((provider, client) =>
            {
                var settings = configuration.GetSection(WakaTimeSettings.SectionName).Get<WakaTimeSettings>();

                client.BaseAddress = new Uri(settings!.BaseUrl);

                var authString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{settings.ApiKey}:"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
            });

        return services;
    }

    public static IHostBuilder AddMessaging(this IHostBuilder host, IConfiguration configuration, Assembly assembly)
    {
        return host.UseWolverine(options =>
        {
            options.Discovery.IncludeAssembly(assembly);

            var rabbitUri = configuration.GetConnectionString("RabbitMq")
                            ?? throw new InvalidOperationException("RabbitMq connection string is missing!");

            options.UseRabbitMq(new Uri(rabbitUri)).AutoProvision();

            options.PublishMessage<ActivityLoggedEvent>()
                   .ToRabbitExchange("core-events-exchange");
        });
    }
}
