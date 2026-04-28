using System.Text.Json.Serialization;

namespace Collector.Domain.Models;

public class WakaTimeSummaryResponse
{
    [JsonPropertyName("data")]
    public List<WakaTimeDaySummary> Data { get; set; } = new();
}
