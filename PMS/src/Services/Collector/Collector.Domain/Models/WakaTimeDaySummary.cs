using System.Text.Json.Serialization;

namespace Collector.Domain.Models;

public class WakaTimeDaySummary
{
    [JsonPropertyName("projects")]
    public List<WakaTimeProject> Projects { get; set; } = new();

    [JsonPropertyName("branches")]
    public List<WakaTimeBranch> Branches { get; set; } = new();
}
