using MediatR;

namespace BudgetFlow.Application.Features.BudgetPeriods.Commands.UpdateBudgetPeriod
{
    public record UpdateBudgetPeriodCommand 
    (
        Guid Id,
        decimal AllocatedBudget
    ) : IRequest<UpdateBudgetPeriodResponse>;
}