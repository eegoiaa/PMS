using Core.API.Services;
using Core.Application.Interfaces;

namespace Core.API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddTransient<ITaskNotifier, SignalRTaskNotifier>();
        return services;
    }
}
