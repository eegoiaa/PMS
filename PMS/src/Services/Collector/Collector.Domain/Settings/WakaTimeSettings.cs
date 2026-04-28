namespace Collector.Domain.Settings;

public class WakaTimeSettings
{
    public const string SectionName = "WakaTimeSettings";
    public required string ApiKey { get; init; }
    public required string BaseUrl { get; init; }
    public required string DeveloperEmail { get; init; }
}
