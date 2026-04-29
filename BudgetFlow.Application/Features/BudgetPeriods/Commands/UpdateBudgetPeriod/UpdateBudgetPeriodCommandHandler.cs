using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.BudgetPeriods.Commands.UpdateBudgetPeriod
{
    public class UpdateBudgetPeriodCommandHandler : IRequestHandler<UpdateBudgetPeriodCommand, UpdateBudgetPeriodResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public UpdateBudgetPeriodCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<UpdateBudgetPeriodResponse> Handle(UpdateBudgetPeriodCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            var role = _currentUserService.Role;

            if(role != nameof(UserRole.TenantAdmin) && role != nameof(UserRole.Finance))
                throw new ForbiddenException("Only TenantAdmin and Finance can update budget periods.");

            var budgetPeriod = await _context.BudgetPeriods
                .FirstOrDefaultAsync(b => b.Id == request.Id && b.TenantId == tenantId, cancellationToken);

            if (budgetPeriod is null)
                throw new NotFoundException("BudgetPeriod", request.Id);

            // I cannot reduce the AllocatedBudget by the amount that has actually been spent
            if (request.AllocatedBudget < budgetPeriod.SpentAmount)
                throw new ValidationException([
                    new FluentValidation.Results.ValidationFailure(
                        "AllocatedBudget",
                        $"Allocated budget cannot be less than the already spent amount (${budgetPeriod.SpentAmount:N2}).")
                ]);

            budgetPeriod.AllocatedBudget = request.AllocatedBudget;
            _context.BudgetPeriods.Update(budgetPeriod);
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateBudgetPeriodResponse(budgetPeriod.Id, budgetPeriod.AllocatedBudget);
        }
    }
}