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
    ) : IRequest<CreateExpenseResponse>;

    
}