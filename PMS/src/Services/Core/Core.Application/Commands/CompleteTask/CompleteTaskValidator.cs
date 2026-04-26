using FluentValidation;

namespace Core.Application.Commands.CompleteTask;

public class CompleteTaskValidator : AbstractValidator<CompleteTaskCommand>
{
    public CompleteTaskValidator()
    {
        RuleFor(t => t.TaskId)
            .NotEmpty().WithMessage("Task Id is missing");
    }
}
