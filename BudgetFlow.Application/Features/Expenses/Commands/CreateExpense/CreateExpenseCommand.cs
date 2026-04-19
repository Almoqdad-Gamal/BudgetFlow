using MediatR;

namespace BudgetFlow.Application.Features.Expenses.Commands.CreateExpense
{
    public record CreateExpenseCommand
    (
        string Title,
        decimal Amount,
        string Currency,
        Guid DepartmentId,
        string? Notes
    ) : IRequest<CreateExpenseResult>;

    public record CreateExpenseResult
    (
        Guid ExpenseId,
        string Title,
        decimal Amount,
        string Status
    );
    
}