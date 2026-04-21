namespace Core.Domain.Entities;

public class Developer
{
    public required Guid Id { get; init; }
    public required string FullName { get; init; }
    public double AverageVolatility { get; set; } = 0;
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}
