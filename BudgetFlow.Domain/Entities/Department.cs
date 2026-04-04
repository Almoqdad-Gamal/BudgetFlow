using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal BudgetLimit { get; set; }
        public string Currency { get; set; } = "USD";

        // Tenant Relationship
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        // Navigation Properties
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<BudgetPeriod> BudgetPeriods { get; set; } = new List<BudgetPeriod>();
    }
}