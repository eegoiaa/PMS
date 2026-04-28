namespace Collector.Domain.Settings;

public class WakaSettings
{
    public const string SectionName = "WakaTimeSettings";
    public required string ApiKey { get; init; }
    public required string BaseUrl { get; init; }
}
