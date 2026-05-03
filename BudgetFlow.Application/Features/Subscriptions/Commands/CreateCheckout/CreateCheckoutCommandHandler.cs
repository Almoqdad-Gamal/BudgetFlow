using BudgetFlow.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Subscriptions.Commands.CreateCheckout
{
    public class CreateCheckoutCommandHandler : IRequestHandler<CreateCheckoutCommand, CreateCheckoutResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISubscriptionService _subscriptionService;

        public CreateCheckoutCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, ISubscriptionService subscriptionService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _subscriptionService = subscriptionService;
        }

        public async Task<CreateCheckoutResponse> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            var userId = _currentUserService.UserId;

            // Get the tenantAdmin email so that stripe can send the receipt
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            var checkoutUrl = await _subscriptionService
                .CreateCheckoutSessionAsync(tenantId, user!.Email);

            return new CreateCheckoutResponse(checkoutUrl);
        }
    }
}