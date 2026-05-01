using Core.API.Hubs;
using Core.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Core.API.Services;

public class SignalRTaskNotifier : ITaskNotifier
{
    private readonly IHubContext<TaskHub> _hubContext;

    public SignalRTaskNotifier(IHubContext<TaskHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyTaskUpdatedAsync(Guid taskId, double newFactHours, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.SendAsync("TaskUpdated", taskId, newFactHours, cancellationToken);
    }
}
