using FluentValidation;

namespace BudgetFlow.Application.Features.Expenses.Commands.ReviewExpense
{
    public class ReviewExpenseCommandValidator : AbstractValidator<ReviewExpenseCommand>
    {
        public ReviewExpenseCommandValidator()
        {
            RuleFor(x => x.ExpenseId)
                .NotEmpty().WithMessage("Expense ID is required.");

            RuleFor(x => x.RejectedReason)
                .NotEmpty().WithMessage("Rejection reason is required when rejecting.")
                .MaximumLength(200)
                .When(x => !x.IsApproved);
        }
    }
}