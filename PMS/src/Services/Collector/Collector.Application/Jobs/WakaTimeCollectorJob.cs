using Collector.Application.Interfaces;
using Collector.Domain.Settings;
using Microsoft.CodeAnalysis;
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

    private static readonly Regex TaskKeyRegex = new Regex(@"[A-Z]+-\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public WakaTimeCollectorJob(
        IWakaTimeProvider wakaTimeProvider,
        IMessageBus messageBus,
        ILogger<WakaTimeCollectorJob> logger,
        IOptions<WakaTimeSettings> options)
    {
        _wakaTimeProvider = wakaTimeProvider;
        _messageBus = messageBus;
        _logger = logger;
        _settings = options.Value;
    }

    public async Task SyncDailyTimeAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting WakaTime daily synchronization job...");

        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var summary = await _wakaTimeProvider.FetchTrackedHoursAsync(today, cancellationToken);

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

            var projectSummary = await _wakaTimeProvider.FetchTrackedHoursForProjectAsync(today, project.Name, cancellationToken);
            var projectData = projectSummary.Data.FirstOrDefault();

            if (projectData?.Branches == null || !projectData.Branches.Any()) continue;

            foreach (var branch in projectData.Branches)
            {
                var match = TaskKeyRegex.Match(branch.Name);

                if (match.Success && branch.TotalSeconds > 0)
                {
                    var taskKey = match.Value.ToUpper();
                    var hours = Math.Round(branch.TotalSeconds / 3600.0, 4); // Округляем часы
                    var userEmail = _settings.DeveloperEmail;
                    var eventMsg = new ActivityLoggedEvent(taskKey, userEmail, hours);

                    await _messageBus.PublishAsync(eventMsg);
                    processedTasks++;

                    _logger.LogInformation($"Tracked {hours} hours for task {taskKey} from branch {branch.Name}");
                }
            }
        }
        _logger.LogInformation($"WakaTime job completed. Processed {processedTasks} valid branches.");
    }
}