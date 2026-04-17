using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, UpdateDepartmentResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public UpdateDepartmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<UpdateDepartmentResult> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.Id && d.TenantId == tenantId, cancellationToken);

            if(department is null)
                throw new NotFoundException("Department", request.Id);

            // Check that the new department name is unique unless it is the same old name
            if(department.Name != request.Name)
            {
                var nameExists = await _context.Departments
                    .Where(d => d.Name == request.Name && d.TenantId == tenantId && d.Id != request.Id)
                    .AnyAsync(cancellationToken);

                if(nameExists)
                    throw new ValidationException([
                        new FluentValidation.Results.ValidationFailure(
                            "Name", "A department with this name already exists.")
                    ]);
            }

            department.Name = request.Name;
            department.BudgetLimit = request.BudgetLimit;
            department.Currency = request.Currency;
            department.UpdatedAt = DateTime.UtcNow;

            _context.Departments.Update(department);
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateDepartmentResult
            (
                department.Id,
                department.Name,
                department.BudgetLimit,
                department.Currency
            );
        }
    }
}