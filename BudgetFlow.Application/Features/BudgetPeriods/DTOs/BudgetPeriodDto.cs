namespace BudgetFlow.Application.Features.BudgetPeriods.DTOs
{
    public record BudgetPeriodDto
    (
        Guid Id,
        Guid DepartmentId,
        string DepartmentName,
        int Month,
        int Year,
        decimal AllocatedBudget,
        decimal SpentAmount,
        decimal RemainingBudget,
        decimal SpentPercentage,
        bool IsAlertThresholdReached
    );
}