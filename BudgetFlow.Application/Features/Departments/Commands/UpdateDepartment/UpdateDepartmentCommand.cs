using MediatR;

namespace BudgetFlow.Application.Features.Departments.Commands.UpdateDepartment
{
    public record UpdateDepartmentCommand
    (
        Guid Id,
        string Name,
        decimal BudgetLimit,
        string Currency
    ) : IRequest<UpdateDepartmentResult>;

    public record UpdateDepartmentResult
    (
        Guid DepartmentId,
        string Name,
        decimal BudgetLimit,
        string Currency
    );
}