using Collector.Application.Interfaces;
using Collector.Domain.Entities;
using Collector.Domain.Settings;
using Microsoft.Extensions.Options;
using Octokit;

namespace Collector.Infrastructure.Providers;

public class GitHubProvider : IGitProvider
{
    private readonly GitHubClient _client;
    private readonly GitHubSettings _settings;

    public GitHubProvider(IOptions<GitHubSettings> options)
    {
        _settings = options.Value;

        _client = new GitHubClient(new ProductHeaderValue(_settings.AppName))
        {
            Credentials = new Credentials(_settings.Token)
        };
    }

    public async Task<IEnumerable<RawActivityLog>> FetchRecentCommitsAsync(DateTimeOffset since, CancellationToken cancellationToken)
    {
        var request = new CommitRequest { Since = since };
        var commits = await _client.Repository.Commit.GetAll(_settings.Owner, _settings.Repository, request);
        var rawLogs = new List<RawActivityLog>();

        foreach (var commit in commits)
        {
            rawLogs.Add(new RawActivityLog
            {
                Id = Guid.NewGuid(),
                Source = "GitHub",
                AuthorEmail = commit.Commit.Author.Email,
                CommitMessage = commit.Commit.Message,
                Timestamp = commit.Commit.Author.Date.UtcDateTime,
                IsProcessed = false
            });
        }

        return rawLogs;
    }
}
