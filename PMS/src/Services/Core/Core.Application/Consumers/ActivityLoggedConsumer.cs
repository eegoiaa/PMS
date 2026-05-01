using Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PMS.Shared.Common.Events;

namespace Core.Application.Consumers;

public static class ActivityLoggedConsumer
{
    public static async Task Handle(
        ActivityLoggedEvent message,
        ICoreDbContext dbContext,
        ILogger logger,
        ITaskNotifier notifier,
        CancellationToken cancellationToken
        )
    {
        logger.LogInformation("Received activity for task {TaskKey} from {Email}: +{Hours} hours",
            message.TaskKey, message.AuthorEmail, message.SpentHours);

        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.TaskKey == message.TaskKey, cancellationToken);

        if (task == null)
        {
            logger.LogWarning("Task with key {TaskKey} not found. Ignoring activity.", message.TaskKey);
            return;
        }

        if (task.IsCompleted)
        {
            logger.LogWarning($"Rejected time update for task {task.TaskKey}: Task is already completed.");
            return;
        }

        task.FactHours += message.SpentHours;

        await dbContext.SaveChangesAsync(cancellationToken);

        await notifier.NotifyTaskUpdatedAsync(task.Id, task.FactHours, cancellationToken);

        logger.LogInformation("Task {TaskKey} updated successfully. New FactHours: {FactHours}",
            task.TaskKey, task.FactHours);
    }
}
