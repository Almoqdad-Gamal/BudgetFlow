using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
    }
}