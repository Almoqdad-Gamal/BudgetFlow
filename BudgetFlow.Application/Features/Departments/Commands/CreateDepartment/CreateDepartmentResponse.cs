namespace BudgetFlow.Application.Features.Departments.Commands.CreateDepartment
{
    public record CreateDepartmentResponse
    (
        Guid DepartmentId,
        string Name,
        decimal BudgetLimit,
        string Currency
    );
}