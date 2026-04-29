using BudgetFlow.Application.Features.Departments.DTOs;
using MediatR;

namespace BudgetFlow.Application.Features.Departments.Queries.GetDepartments
{
    public record GetDepartmentsQuery() : IRequest<List<DepartmentDto>>;
    
    
    
}