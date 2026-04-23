namespace Collector.Domain.Entities;

public class RawActivityLog
{
    public required Guid Id { get; init; }
    public required string Source { get; init; } // откуда получаем данные 
    public required string AuthorEmail { get; init; }  
    public required string CommitMessage { get; init; } 
    public DateTime Timestamp { get; init; } // временная метка
    public bool IsProcessed { get; set; }
}
