namespace BudgetFlow.Application.Features.Expenses.DTOs
{
    public record ExpenseDto
    (
        Guid Id,
        string Title,
        decimal Amount,
        string Currency,
        string Status,
        string CreatedByFullName,
        Guid DepartmentId,
        string DepartmentName,
        DateTime CreatedAt
    );
}