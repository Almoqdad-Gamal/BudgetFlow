using FluentValidation;

namespace BudgetFlow.Application.Features.BudgetPeriods.Commands.CreateBudgetPeriod
{
    public class CreateBudgetPeriodCommandValidator : AbstractValidator<CreateBudgetPeriodCommand>
    {
        public CreateBudgetPeriodCommandValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department is required.");

            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12");

            RuleFor(x => x.Year)
                .InclusiveBetween(2026, 2100).WithMessage("Year is not valid.");

            RuleFor(x => x.AllocatedBudget)
                .GreaterThan(0).WithMessage("Allocated budget must be greater than 0");
        }
    }
}