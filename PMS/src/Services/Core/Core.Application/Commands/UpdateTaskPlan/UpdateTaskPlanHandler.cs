using Core.Application.Interfaces;
using Core.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using PMS.Shared.Common.Exceptions;

namespace Core.Application.Commands.UpdateTaskPlan;

public static class UpdateTaskPlanHandler
{
    public static async Task Handle(
        UpdateTaskPlanCommand command,
        ICoreDbContext dbContext,
        CancellationToken cancellationToken
        )
    {
        var task = await dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == command.TaskId, cancellationToken)
            ?? throw new NotFoundException($"Task with ID {command.TaskId} was not found.");

        if (task.IsCompleted)
            throw new CoreException("Cannot update plan hours for a completed task.");

        task.PlanHours = command.NewPlanHours;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
