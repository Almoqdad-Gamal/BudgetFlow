using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, CreateDepartmentResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateDepartmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<CreateDepartmentResult> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // Check that the department name is unique within the same tenant
            var DepartmentExists = await _context.Departments
                .Where(d => d.Name == request.Name && d.TenantId == tenantId)
                .AnyAsync(cancellationToken);

            if(DepartmentExists)
                throw new ValidationException([
                    new FluentValidation.Results.ValidationFailure(
                        "Name", "A department with this name already exists in your organization.")
                ]);

            // Check that the tenant has not exceeded the maximum number of departments
            var DepartmentCount = await _context.Departments
                .Where(d => d.TenantId == tenantId)
                .CountAsync(cancellationToken);

            var tenant = await  _context.Tenants.FindAsync(
                new object[] {tenantId}, cancellationToken:cancellationToken);

            if(DepartmentCount >= tenant!.MaxDepartments)
                throw new ForbiddenException($"You have reached the maximum number of departments ({tenant.MaxDepartments}) for your plan.");

            var department = new Department
            {
                Name = request.Name,
                BudgetLimit = request.BudgetLimit,
                Currency = request.Currency,
                TenantId = tenantId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateDepartmentResult(
                department.Id,
                department.Name,
                department.BudgetLimit,
                department.Currency
            );
        }
    }
}