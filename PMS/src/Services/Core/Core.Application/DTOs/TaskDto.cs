namespace Core.Application.DTOs;

public record TaskDto(
    Guid Id,
    string TaskKey,
    string Title,
    double PlanHours,
    double FactHours,
    bool IsCompleted,
    Guid DeveloperId
    );
