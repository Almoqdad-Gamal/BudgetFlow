using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetFlow.Application.Features.Subscriptions.Commands.CreateCheckout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "TenantAdmin")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISender _sender;
        public SubscriptionsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CreateCheckout(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateCheckoutCommand(), cancellationToken);

            return Ok(result);
        }
    }
}