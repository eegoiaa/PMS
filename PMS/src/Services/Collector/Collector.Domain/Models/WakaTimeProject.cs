using System.Text.Json.Serialization;

namespace Collector.Domain.Models;

public class WakaTimeProject
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}
