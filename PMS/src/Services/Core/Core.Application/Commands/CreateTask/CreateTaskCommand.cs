namespace Core.Application.Commands.CreateTask;

public record CreateTaskCommand(
    string TaskKey,
    Guid DeveloperId,
    string Title,
    double PlanHours
);
