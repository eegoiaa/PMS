using FluentValidation;

namespace Core.Application.Commands.CreateTask;

public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(t => t.DeveloperId)
            .NotEmpty().WithMessage("Developer Id is missing");

        RuleFor(t => t.Title)
            .NotEmpty().WithMessage("Title is missing")
            .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

        RuleFor(t => t.PlanHours)
            .InclusiveBetween(0.1, 200.0)
            .WithMessage("Plan hours must be between 0.1 and 200 hours");
    }
}
