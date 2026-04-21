namespace Core.Application.Commands.CreateTask;

public record CreateTaskCommand(
    Guid DeveloperId,
    string Title,
    double PlanHours
);
