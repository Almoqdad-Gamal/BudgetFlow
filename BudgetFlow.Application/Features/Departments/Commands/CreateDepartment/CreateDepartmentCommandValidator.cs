using FluentValidation;

namespace BudgetFlow.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.BudgetLimit)
                .GreaterThan(0).WithMessage("Budget limit must be greater than 0.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3).WithMessage("Currency code must be exactly 3 characters (e.g., USD, EUR)");
        }
    }
}