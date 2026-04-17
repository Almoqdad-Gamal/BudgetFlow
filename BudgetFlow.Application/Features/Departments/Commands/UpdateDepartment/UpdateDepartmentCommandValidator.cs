using FluentValidation;

namespace BudgetFlow.Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.BudgetLimit)
                .GreaterThan(0).WithMessage("Budget limit must me greater than 0.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3).WithMessage("Currency code must be exactly 3 characters.");
        }
    }
}