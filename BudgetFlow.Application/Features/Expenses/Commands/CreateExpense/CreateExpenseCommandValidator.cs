using FluentValidation;

namespace BudgetFlow.Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
    {
        public CreateExpenseCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3).WithMessage("Currency must be 3 characters (e.g., USD, EUR).");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department is required");

            RuleFor(x => x.Notes)
                .MaximumLength(200).When(x => x.Notes is not null);
        }
    }
}