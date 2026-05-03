using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace BudgetFlow.Infrastructure.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly StripeSettings _settings;
        public SubscriptionService(IOptions<StripeSettings> settings)
        {
            _settings = settings.Value;
            StripeConfiguration.ApiKey = _settings.SecretKey;
        }

        public async Task<string> CreateCheckoutSessionAsync(Guid tenantId, string tenantEmail)
        {
            var options = new SessionCreateOptions
            {
                Mode = "Subscription",

                CustomerEmail = tenantEmail,

                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Price = _settings.ProPlanPriceId,
                        Quantity = 1
                    }
                ],

                SuccessUrl = "https://budgetflow.com/upgrade/success",

                CancelUrl = "https://budgetflow.com/upgrade/cancel",

                Metadata = new Dictionary<string, string>
                {
                    {"tenantId", tenantId.ToString()}
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }
    }
}