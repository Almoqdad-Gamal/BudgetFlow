using MediatR;

namespace BudgetFlow.Application.Features.Subscriptions.Commands.UpgradeToPro
{
    public record UpgradeToProCommand (Guid TenantId) : IRequest<Unit>;
}