namespace Core.Application.Commands.UpdateTaskPlan;

public record UpdateTaskPlanCommand(
    Guid TaskId,
    double NewPlanHours
    );
