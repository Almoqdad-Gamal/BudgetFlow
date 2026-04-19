using MediatR;

namespace BudgetFlow.Application.Features.Expenses.Commands.ReviewExpense
{
    public record ReviewExpenseCommand 
    (
        Guid ExpenseId,
        bool IsApproved,
        string? RejectedReason = null
    ) : IRequest<ReviewExpenseResponse>;
}