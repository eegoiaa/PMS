using Collector.Application.Interfaces;
using Collector.Domain.Entities;
using System.Text.RegularExpressions;

namespace Collector.Infrastructure.Services;

public class ActivityCleaner : IActivityCleaner
{
    private static readonly Regex TaskKeyRegex = new Regex(
        @"[A-Z]+-\d+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Task<string?> ExtractTaskIdAsync(RawActivityLog rawLog, CancellationToken cancellationToken)
    {
        var match = TaskKeyRegex.Match(rawLog.CommitMessage);

        if (match.Success)
            return Task.FromResult<string?>(match.Value);
        
        return Task.FromResult<string?>(null);
    }
}
