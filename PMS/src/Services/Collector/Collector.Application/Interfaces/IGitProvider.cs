using Collector.Domain.Entities;

namespace Collector.Application.Interfaces;

public interface IGitProvider
{
    Task<IEnumerable<RawActivityLog>> FetchRecentCommitsAsync(DateTimeOffset since, CancellationToken cancellationToken);
}
