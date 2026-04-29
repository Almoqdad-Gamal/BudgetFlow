namespace BudgetFlow.Application.Features.BudgetPeriods.Commands.UpdateBudgetPeriod
{
    public record UpdateBudgetPeriodResponse
    (
        Guid Id,
        decimal AllocatedBudget
    );
}