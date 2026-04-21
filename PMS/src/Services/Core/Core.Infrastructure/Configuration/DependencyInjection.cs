using Core.Application.Interfaces;
using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Core.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CoreDbContext>(options =>
            options.UseNpgsql(connectionString,
                x => x.MigrationsAssembly(typeof(CoreDbContext).Assembly.GetName().Name)
            )
        );

        services.AddScoped<ICoreDbContext>(provider => provider.GetRequiredService<CoreDbContext>());

        return services;
    }

    public static IHostBuilder AddMessaging(this IHostBuilder host, IConfiguration configuration, Assembly assembly)
    {
        return host.UseWolverine(options =>
        {
            var rabbitUri = configuration.GetConnectionString("RabbitMq")
                            ?? throw new InvalidOperationException("RabbitMq connection string is missing!");

            options.UseRabbitMq(new Uri(rabbitUri)).AutoProvision();
            options.ListenToRabbitQueue("core-events-queue");
            options.Policies.AutoApplyTransactions();

            options.Discovery.IncludeAssembly(assembly);
        });
    }
}
