namespace Core.Domain.Entities;

public class ProjectTask
{
    public required Guid Id { get; init; }

    public required string TaskKey { get; init; }
    public required string Title { get; init; }
    public required Guid DeveloperId { get; init; }
    public Developer? Developer { get; set; }

    public double PlanHours { get; set; }
    public double FactHours { get; set; }
    public bool IsCompleted { get; set; }
}
