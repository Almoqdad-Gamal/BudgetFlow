using BudgetFlow.Application.Features.BudgetPeriods.Commands.CreateBudgetPeriod;
using BudgetFlow.Application.Features.BudgetPeriods.Queries.GetBudgetPeriods;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetPeriodsController : ControllerBase
    {
        private readonly ISender _sender;
        public BudgetPeriodsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles ="TenantAdmin,Finance")]
        public async Task<IActionResult> CreateBudgetPeriod([FromBody] CreateBudgetPeriodCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetBudgetPeriods), result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBudgetPeriods([FromQuery] Guid? departmentId, [FromQuery] int? month, [FromQuery] int? year, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetBudgetPeriodsQuery(departmentId, month, year),
                cancellationToken);
            return Ok(result);
        }
    }
}