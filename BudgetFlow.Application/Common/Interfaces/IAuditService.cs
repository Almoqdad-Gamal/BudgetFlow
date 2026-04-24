namespace BudgetFlow.Application.Common.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync (
            string action,
            string entityName,
            string? oldValues = null,
            string? newValues = null,
            CancellationToken cancellationToken = default
        );
    }
}