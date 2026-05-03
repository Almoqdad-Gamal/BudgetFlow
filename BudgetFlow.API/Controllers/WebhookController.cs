using BudgetFlow.Application.Features.Subscriptions.Commands.UpgradeToPro;
using BudgetFlow.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace BudgetFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly StripeSettings _settings;
        private readonly ILogger<WebhookController> _logger;
        public WebhookController(ISender sender, IOptions<StripeSettings> settings, ILogger<WebhookController> logger)
        {
            _sender = sender;
            _settings = settings.Value;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            // Read the row body from the request (because stripe send raw json not json)
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                // Check if the request is actually coming from stripe
                // If there is any one else send request to webhook it will reject
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _settings.WebhookSecret
                );

                // Handle the event
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    if(session?.Metadata.TryGetValue("tenantId", out var tenantIdStr) == true
                        && Guid.TryParse(tenantIdStr, out var tenantId))
                    {
                        await _sender.Send(new UpgradeToProCommand(tenantId));
                    }
                }
                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error");
                return BadRequest();
            }
        }
    }
}