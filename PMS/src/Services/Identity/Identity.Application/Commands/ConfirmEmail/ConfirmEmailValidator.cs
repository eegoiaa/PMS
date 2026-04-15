using FluentValidation;

namespace Identity.Application.Commands.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
	public ConfirmEmailValidator()
	{
		RuleFor(x => x.UserId)
			.NotEmpty().WithMessage("User ID is required");

		RuleFor(x => x.Token)
			.NotEmpty().WithMessage("Confirmation token is required");
	}
}
