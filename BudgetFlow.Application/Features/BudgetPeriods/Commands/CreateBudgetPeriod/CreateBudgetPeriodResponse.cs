namespace BudgetFlow.Application.Features.BudgetPeriods.Commands.CreateBudgetPeriod
{
    public record CreateBudgetPeriodResponse
    (
        Guid BudgetPeriodId,
        Guid DepartmentId,
        int Month,
        int Year,
        decimal AllocatedBudget
    );
}