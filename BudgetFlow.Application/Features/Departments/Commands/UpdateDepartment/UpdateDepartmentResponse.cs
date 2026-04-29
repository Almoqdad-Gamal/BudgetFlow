namespace BudgetFlow.Application.Features.Departments.Commands.UpdateDepartment
{
    public record UpdateDepartmentResponse
    (
        Guid DepartmentId,
        string Name,
        decimal BudgetLimit,
        string Currency
    );
}