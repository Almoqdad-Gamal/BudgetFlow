using MediatR;

namespace BudgetFlow.Application.Features.Departments.Commands.DeleteDepartment
{
    public record DeleteDepartmentCommand (Guid Id) : IRequest<Unit>;

}