using BudgetFlow.Domain.Enums;
using FluentValidation;

namespace BudgetFlow.Application.Features.Users.Commands.AddUser
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        private static readonly string[] AllowedRoles =
        [
            nameof(UserRole.Manager),
            nameof(UserRole.Finance),
            nameof(UserRole.Employee)
        ];
        public AddUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Must contain at least one uppercase letter.")
                .Matches("[0-9]").WithMessage("Must contain at least one number.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(role => AllowedRoles.Contains(role))
                .WithMessage($"Role must be one of: {string.Join(", ", AllowedRoles)}");
        }
    }
}