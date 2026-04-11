using FluentValidation;

namespace BudgetFlow.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.TenantName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Subdomain)
                .NotEmpty()
                .MaximumLength(50)
                .Matches("^[a-z0-9-]+$")
                .WithMessage("Subdomain must be lowercase letters, numbers, or hyphens only.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Must contain at least one uppercase letter.")
                .Matches("[0-9]").WithMessage("Must contain at least on number.");

            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        }
    }
}