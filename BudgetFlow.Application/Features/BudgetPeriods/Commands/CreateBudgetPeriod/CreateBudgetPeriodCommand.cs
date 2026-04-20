using MediatR;

namespace BudgetFlow.Application.Features.BudgetPeriods.Commands.CreateBudgetPeriod
{
    public record CreateBudgetPeriodCommand
    (
        Guid DepartmentId,
        int Month,
        int Year,
        decimal AllocatedBudget
    ) : IRequest<CreateBudgetPeriodResponse>;
}