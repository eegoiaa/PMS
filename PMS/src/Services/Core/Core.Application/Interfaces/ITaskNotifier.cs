namespace Core.Application.Interfaces;

public interface ITaskNotifier
{
    Task NotifyTaskUpdatedAsync(Guid taskId, double newFactHours, CancellationToken cancellationToken);
}
