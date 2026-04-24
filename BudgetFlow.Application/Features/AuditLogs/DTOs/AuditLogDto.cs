namespace BudgetFlow.Application.Features.AuditLogs.DTOs
{
    public record AuditLogDto
    (
        Guid Id,
        string Action,
        string EntityName,
        string? OldValues,
        string? NewValues,
        string? IpAddress,
        DateTime CreatedAt
    );
}