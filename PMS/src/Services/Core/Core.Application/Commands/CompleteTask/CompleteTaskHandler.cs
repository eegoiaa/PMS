using Core.Application.Interfaces;
using Core.Domain.Exceptions;
using Core.Domain.Logic;
using Microsoft.EntityFrameworkCore;
using PMS.Shared.Common.Exceptions;

namespace Core.Application.Commands.CompleteTask;

public static class CompleteTaskHandler
{
    public static async Task Handle(
        CompleteTaskCommand command,
        ICoreDbContext dbContext,
        CancellationToken cancellationToken
        )
    {
        var task = await dbContext.Tasks
            .Include(t => t.Developer)
            .ThenInclude(d => d.Tasks)
            .FirstOrDefaultAsync(t => t.Id == command.TaskId, cancellationToken)
            ?? throw new NotFoundException($"Task with ID {command.TaskId} was not found.");

        if (task.IsCompleted)
            throw new CoreException("This task is already completed. You cannot complete it again.");

        task.IsCompleted = true;

        var developer = task.Developer;
        if (developer != null)
        {
            var completedTasks = developer.Tasks.Where(t => t.IsCompleted == true).ToList();
            
            if (completedTasks.Any())
            {
                var totalVolatility = completedTasks.Sum(t => VolatilityCalculator.CalculateTaskVolatility(t));
                developer.AverageVolatility = totalVolatility / completedTasks.Count;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
