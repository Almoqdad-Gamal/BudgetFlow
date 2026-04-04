using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class BudgetPeriod : BaseEntity
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal AllocatedBudget { get; set; }
        public decimal SpentAmount { get; set; } = 0;

        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        // Not saved in DB

        public decimal RemainingBudget => AllocatedBudget - SpentAmount;
        public decimal SpentPercentage => AllocatedBudget == 0 ? 0 : (SpentAmount / AllocatedBudget) * 100;
        public bool IsAlertThresholdReached => SpentPercentage >= 80;
    }
}