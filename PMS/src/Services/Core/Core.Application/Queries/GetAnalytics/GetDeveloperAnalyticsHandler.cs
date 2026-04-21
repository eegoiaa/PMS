using Core.Application.Interfaces;
using Core.Domain.Logic;
using Microsoft.EntityFrameworkCore;
using PMS.Shared.Common.Exceptions;

namespace Core.Application.Queries.GetAnalytics;

public static class GetDeveloperAnalyticsHandler
{
    public static async Task<GetDeveloperAnalyticsDto> Handle(
        GetDeveloperAnalyticsQuery query,
        ICoreDbContext dbContext,
        CancellationToken cancellationToken
        )
    {
        var developer = await dbContext.Developers
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(d => d.Id == query.DeveloperId) ?? throw new NotFoundException($"Developer with ID {query.DeveloperId} was not found.");

        var activeTasks = developer.Tasks
            .Where(t => !t.IsCompleted)
            .ToList();

        var predictedLoad = VolatilityCalculator.CalculateTotalPredictedLoad(activeTasks, developer.AverageVolatility);

        return new GetDeveloperAnalyticsDto(
            developer.FullName,
            developer.AverageVolatility,
            predictedLoad,
            activeTasks.Count
        );
    }
}
