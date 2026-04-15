using FluentValidation;
using Identity.Domain.Constants;

namespace Identity.Application.Commands.SignUp;

public class SignUpValidator : AbstractValidator<SignUpCommand>
{
    public SignUpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please, enter the email")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("You've entered an invalid email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Please, enter the password")
            .Matches(@"[0-9]").When(_ => AuthConstants.RequireDigit)
                .WithMessage("Password must contain at least one digit")
            .Matches(@"[A-Z]").When(_ => AuthConstants.RequireUppercase)
                .WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").When(_ => AuthConstants.RequireLowercase)
                .WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[^a-zA-Z0-9]").When(_ => AuthConstants.RequireNonAlphanumeric)
                .WithMessage("Password must contain at least one special character");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Please, reenter the password")
            .Equal(x => x.Password).WithMessage("The passwords you’ve entered don’t coincide");

        RuleFor(x => x.FullName).NotEmpty().WithMessage("Please, enter your full name");
    }
}
