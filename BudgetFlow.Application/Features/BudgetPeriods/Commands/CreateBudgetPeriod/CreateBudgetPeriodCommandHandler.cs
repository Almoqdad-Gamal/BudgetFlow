using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.BudgetPeriods.Commands.CreateBudgetPeriod
{
    public class CreateBudgetPeriodCommandHandler : IRequestHandler<CreateBudgetPeriodCommand, CreateBudgetPeriodResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateBudgetPeriodCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<CreateBudgetPeriodResponse> Handle(CreateBudgetPeriodCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            var role = _currentUserService.Role;

            // Check the role 
            if (role != "TenantAdmin" && role != "Finance")
                throw new ForbiddenException("Only TenantAdmin and Finance can create budget periods.");

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.TenantId == tenantId, cancellationToken);

            if(department is null)
                throw new NotFoundException("Department", request.DepartmentId);

            // Check that there is no BudgetPeriod for the same department and month and year
            var periodExists = await _context.BudgetPeriods
                .AnyAsync(b => 
                    b.DepartmentId == request.DepartmentId &&
                    b.Month == request.Month &&
                    b.Year == request.Year,
                    cancellationToken
                );

            if(periodExists)
                throw new ValidationException([
                    new FluentValidation.Results.ValidationFailure(
                        "Period", $"A budget period for {request.Month}/{request.Year} already exists for this department.")
                ]);

            var budgetPeriod = new BudgetPeriod
            {
                DepartmentId = request.DepartmentId,
                TenantId = tenantId,
                Month = request.Month,
                Year = request.Year,
                AllocatedBudget = request.AllocatedBudget,
                SpentAmount = 0
            };

            _context.BudgetPeriods.Add(budgetPeriod);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateBudgetPeriodResponse
            (
                budgetPeriod.Id,
                budgetPeriod.DepartmentId,
                budgetPeriod.Month,
                budgetPeriod.Year,
                budgetPeriod.AllocatedBudget
            );
        }
    }
}