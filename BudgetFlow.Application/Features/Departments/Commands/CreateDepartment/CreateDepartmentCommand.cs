using MediatR;

namespace BudgetFlow.Application.Features.Departments.Commands.CreateDepartment
{
    public record CreateDepartmentCommand
    (
        string Name,
        decimal BudgetLimit,
        string Currency = "USD"
    ) : IRequest<CreateDepartmentResult>;

    public record CreateDepartmentResult
    (
        Guid DepartmentId,
        string Name,
        decimal BudgetLimit,
        string Currency
    );
}