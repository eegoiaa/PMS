namespace Collector.Application.Interfaces;

using Collector.Domain.Models;
using Refit;

public interface IWakaTimeProvider
{
    [Get("/users/current/summaries?start={date}&end={date}")]
    Task<WakaTimeSummaryResponse> FetchTrackedHoursAsync(string date, CancellationToken cancellationToken);

    [Get("/users/current/summaries?start={date}&end={date}&project={project}")]
    Task<WakaTimeSummaryResponse> FetchTrackedHoursForProjectAsync(string date, string project, CancellationToken cancellationToken);
}
