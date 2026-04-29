namespace BudgetFlow.Application.Features.Departments.DTOs
{
    public record DepartmentDto(
        Guid Id,
        string Name,
        decimal BudgetLimit,
        string Currency
    );
}