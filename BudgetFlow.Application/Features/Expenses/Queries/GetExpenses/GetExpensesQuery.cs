using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Features.Expenses.DTOs;
using MediatR;

namespace BudgetFlow.Application.Features.Expenses.Queries.GetExpenses
{
    public record GetExpensesQuery
    (
        Guid? DepartmentId = null,
        string? Status = null,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<PagedResult<ExpenseDto>>;

    
    
}