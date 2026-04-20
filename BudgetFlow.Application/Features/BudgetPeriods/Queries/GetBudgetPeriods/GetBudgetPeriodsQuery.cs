using BudgetFlow.Application.Features.BudgetPeriods.DTOs;
using MediatR;

namespace BudgetFlow.Application.Features.BudgetPeriods.Queries.GetBudgetPeriods
{
    public record GetBudgetPeriodsQuery
    (
        Guid? DepartmentId = null,
        int? Month = null,
        int? Year = null
    ) : IRequest<List<BudgetPeriodDto>>;
}