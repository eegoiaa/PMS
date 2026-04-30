using Collector.Application.Interfaces;
using Collector.Domain.Entities;
using Collector.Domain.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.Shared.Common.Events;
using System.Text.RegularExpressions;
using Wolverine;

namespace Collector.Application.Jobs;

public class WakaTimeCollectorJob
{
    private readonly IWakaTimeProvider _wakaTimeProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<WakaTimeCollectorJob> _logger;
    private readonly WakaTimeSettings _settings;
    private readonly ICollectorDbContext _dbContext;

    private static readonly Regex TaskKeyRegex = new Regex(@"[A-Z]+-\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public WakaTimeCollectorJob(
        IWakaTimeProvider wakaTimeProvider,
        IMessageBus messageBus,
        ILogger<WakaTimeCollectorJob> logger,
        IOptions<WakaTimeSettings> options,
        ICollectorDbContext dbContext)
    {
        _wakaTimeProvider = wakaTimeProvider;
        _messageBus = messageBus;
        _logger = logger;
        _settings = options.Value;
        _dbContext = dbContext;
    }

    public async Task SyncDailyTimeAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting WakaTime daily synchronization job...");

        var todayDate = DateTime.UtcNow.Date;
        var todayStr = todayDate.ToString("yyyy-MM-dd");

        var summary = await _wakaTimeProvider.FetchTrackedHoursAsync(todayStr, cancellationToken);

        if (summary.Data == null || !summary.Data.Any())
        {
            _logger.LogWarning("WakaTime API returned empty data for today.");
            return;
        }

        var todayData = summary.Data.First();

        if (todayData.Projects == null || !todayData.Projects.Any())
        {
            _logger.LogWarning("No projects found in WakaTime for today.");
            return;
        }

        int processedTasks = 0;

        foreach (var project in todayData.Projects)
        {
            _logger.LogInformation($"Found project '{project.Name}'. Fetching branches for this project...");

            var projectSummary = await _wakaTimeProvider.FetchTrackedHoursForProjectAsync(todayStr, project.Name, cancellationToken);
            var projectData = projectSummary.Data.FirstOrDefault();

            if (projectData?.Branches == null || !projectData.Branches.Any()) continue;

            foreach (var branch in projectData.Branches)
            {
                var match = TaskKeyRegex.Match(branch.Name);

                if (match.Success && branch.TotalSeconds > 0)
                {
                    var taskKey = match.Value.ToUpper();

                    var syncRecord = await _dbContext.WakaTimeSyncs.
                        FirstOrDefaultAsync(w => w.TaskKey == taskKey && w.Date == todayDate, cancellationToken);

                    if (syncRecord == null)
                    {
                        syncRecord = new WakaTimeSync { Id = Guid.NewGuid(), TaskKey = taskKey, Date = todayDate, LastSentSeconds = 0 };
                        _dbContext.WakaTimeSyncs.Add(syncRecord);
                    }

                    double newSeconds = branch.TotalSeconds - syncRecord.LastSentSeconds;

                    if (newSeconds > 0)
                    {
                        var deltaHours = Math.Round(newSeconds / 3600.0, 4);
                        var userEmail = _settings.DeveloperEmail;
                        var eventMsg = new ActivityLoggedEvent(taskKey, userEmail, deltaHours);

                        await _messageBus.PublishAsync(eventMsg);

                        syncRecord.LastSentSeconds = branch.TotalSeconds;

                        _logger.LogInformation($"Tracked {deltaHours} hours for task {taskKey} from branch {branch.Name}");
                    }  
                }
            }
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"WakaTime job completed. Processed {processedTasks} valid branches.");
    }
}