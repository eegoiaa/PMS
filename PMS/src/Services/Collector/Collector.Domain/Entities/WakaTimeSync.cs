namespace Collector.Domain.Entities;

public class WakaTimeSync
{
    public Guid Id { get; set; }
    public required string TaskKey { get; set; }
    public DateTime Date { get; set; }
    public double LastSentSeconds { get; set; }
}
