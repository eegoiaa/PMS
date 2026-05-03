using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PMS.Shared.Common.Exceptions;

namespace Core.Application.Commands.CreateTask;

public static class CreateTaskHandler
{
    public static async Task<Guid> Handle(
        CreateTaskCommand command,
        ICoreDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var developerExists = dbContext.Developers.Any(d => d.Id == command.DeveloperId);
        if (!developerExists)
            throw new NotFoundException($"Developer with ID {command.DeveloperId} was not found.");

        var tasksCount = await dbContext.Tasks
            .CountAsync(t => t.DeveloperId == command.DeveloperId, cancellationToken);

        var generatedTaskKey = $"TASK-{tasksCount + 1}";

        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            TaskKey = generatedTaskKey,
            Title = command.Title,
            DeveloperId = command.DeveloperId,
            PlanHours = command.PlanHours,
            FactHours = 0,
            IsCompleted = false
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}
