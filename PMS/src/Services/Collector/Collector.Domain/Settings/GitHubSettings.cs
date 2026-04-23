namespace Collector.Domain.Settings;

public class GitHubSettings
{
    public const string SectionName = "GitHubSettings";
    public required string Token { get; init; }
    public required string Owner { get; init; }
    public required string Repository { get; init; }
    public required string AppName { get; init; }
}
