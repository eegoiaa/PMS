using System.Text.Json.Serialization;

namespace Collector.Domain.Models;

public class WakaTimeBranch
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("total_seconds")]
    public double TotalSeconds { get; set; }
}
