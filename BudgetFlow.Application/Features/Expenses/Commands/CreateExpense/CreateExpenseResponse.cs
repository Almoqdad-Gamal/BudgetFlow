namespace BudgetFlow.Application.Features.Expenses.Commands.CreateExpense
{
    public record CreateExpenseResponse
    (
        Guid ExpenseId,
        string Title,
        decimal Amount,
        string Status
    );
}