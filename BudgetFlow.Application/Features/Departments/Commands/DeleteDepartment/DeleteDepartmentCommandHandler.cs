using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DeleteDepartmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.Id && d.TenantId == tenantId, cancellationToken);

            if (department is null)
                throw new NotFoundException("Department", request.Id);

            // Check that the department doesn't have expenses before delete
            var hasExpenses = await _context.Expenses
                .AnyAsync(e => e.DepartmentId == request.Id, cancellationToken);

            if(hasExpenses)
                throw new ForbiddenException("Cannot delete a department that has expenses. Please delete all expenses first.");

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}