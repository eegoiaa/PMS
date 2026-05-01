using Core.Application.DTOs;
using Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Queries.GetMyTasks;

public static class GetMyTasksHandler
{
    public static async Task<IEnumerable<TaskDto>> Handle(
        GetMyTasksQuery query,
        ICoreDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tasks
            .Where(t => t.DeveloperId == query.DeveloperId)
            .Select(t => new TaskDto(
                t.Id,
                t.TaskKey,
                t.Title,
                t.PlanHours,
                t.FactHours,
                t.IsCompleted,
                t.DeveloperId
            ))
            .ToListAsync(cancellationToken);
    }
}
