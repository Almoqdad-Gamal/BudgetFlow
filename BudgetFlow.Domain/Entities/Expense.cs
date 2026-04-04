using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities
{
    public class Expense : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal AmountInBaseCurrency { get; set; }
        public string? Notes { get; set; }
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

        // Who submitted it
        public Guid CreatedByUserId { get; set; }
        public AppUser CreatedByUser { get; set; } = null!;

        // Who approved/rejected it
        public Guid? ReviewedByUserId { get; set; }
        public AppUser? ReviewedByUser { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectionReason { get; set; }

        // Where it belongs
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}