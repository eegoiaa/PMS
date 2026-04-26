using FluentValidation;

namespace Core.Application.Commands.UpdateTaskPlan;

public class UpdateTaskPlanValidator : AbstractValidator<UpdateTaskPlanCommand>
{
    public UpdateTaskPlanValidator()
    {
        RuleFor(t => t.TaskId)
            .NotEmpty().WithMessage("Task ID is missing");

        RuleFor(t => t.NewPlanHours)
            .InclusiveBetween(0.1, 200.0)
            .WithMessage("Plan hours must be between 0.1 and 200 hours");
    }
}
