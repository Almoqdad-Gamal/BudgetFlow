using MediatR;

namespace BudgetFlow.Application.Features.Departments.Queries.GetDepartments
{
    public record GetDepartmentsQuery() : IRequest<List<DepartmentDto>>;
    
    public record DepartmentDto(
        Guid Id,
        string Name,
        decimal BudgetLimit,
        string Currency
    );
    
}