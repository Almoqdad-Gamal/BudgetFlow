using BudgetFlow.Application.Features.Expenses.DTOs;
using MediatR;

namespace BudgetFlow.Application.Features.Expenses.Queries.GetExpenses
{
    public record GetExpensesQuery
    (
        Guid? DepartmentId = null,
        string? Status = null
    ) : IRequest<List<ExpenseDto>>;

    
    
}