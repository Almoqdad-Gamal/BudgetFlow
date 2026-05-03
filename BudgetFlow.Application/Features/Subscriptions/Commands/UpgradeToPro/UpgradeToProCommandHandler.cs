using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Subscriptions.Commands.UpgradeToPro
{
    public class UpgradeToProCommandHandler : IRequestHandler<UpgradeToProCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpgradeToProCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpgradeToProCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

            if (tenant is null)
                throw new NotFoundException("Tenant", request.TenantId);

            tenant.Plan = SubscriptionPlan.Pro;
            tenant.MaxDepartments = int.MaxValue;
            tenant.MaxUsers = int.MaxValue;

            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}