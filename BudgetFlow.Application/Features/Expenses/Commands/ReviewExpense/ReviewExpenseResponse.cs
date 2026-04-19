namespace BudgetFlow.Application.Features.Expenses.Commands.ReviewExpense
{
    public record ReviewExpenseResponse
    (
        Guid ExpenseId,
        string NewStatus
    );
}