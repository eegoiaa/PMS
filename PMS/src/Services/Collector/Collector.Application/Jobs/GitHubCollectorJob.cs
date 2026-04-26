using Collector.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PMS.Shared.Common.Events;
using Wolverine;

namespace Collector.Application.Jobs;

public class GitHubCollectorJob
{
    private readonly IGitProvider _gitProvider;
    private readonly IActivityCleaner _activityCleaner;
    private readonly ICollectorDbContext _dbContext;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<GitHubCollectorJob> _logger;

    public GitHubCollectorJob(
        IGitProvider gitProvider,
        IActivityCleaner activityCleaner,
        ICollectorDbContext dbContext,
        IMessageBus messageBus,
        ILogger<GitHubCollectorJob> logger)
    {
        _gitProvider = gitProvider;
        _activityCleaner = activityCleaner;
        _dbContext = dbContext;
        _messageBus = messageBus;
        _logger = logger;
    }

    // Это метод, который будет дергать Hangfire
    public async Task SyncCommitsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting GitHub synchronization job via Hangfire...");

        var since = DateTimeOffset.UtcNow.AddDays(-7);
        var recentCommits = await _gitProvider.FetchRecentCommitsAsync(since, cancellationToken);

        int newActivitiesCount = 0;

        foreach (var commit in recentCommits)
        {
            // Дедупликация
            var exists = await _dbContext.RawActivityLogs
                .AnyAsync(l => l.CommitMessage == commit.CommitMessage && l.Timestamp == commit.Timestamp, cancellationToken);

            if (exists) continue;

            // Чистим и ищем TaskKey
            var taskKey = await _activityCleaner.ExtractTaskIdAsync(commit, cancellationToken);

            if (taskKey != null)
            {
                var eventMsg = new ActivityLoggedEvent(taskKey, commit.AuthorEmail, 15.0);
                await _messageBus.PublishAsync(eventMsg);

                commit.IsProcessed = true;
                newActivitiesCount++;
            }
            else
            {
                commit.IsProcessed = false;
            }

            _dbContext.RawActivityLogs.Add(commit);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Job completed. Found {recentCommits.Count()} commits, processed {newActivitiesCount} valid tasks.");
    }
    //fsfs
}
