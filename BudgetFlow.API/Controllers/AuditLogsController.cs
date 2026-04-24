using BudgetFlow.Application.Features.AuditLogs.Queries.GetAuditLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "TenantAdmin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly ISender _sender;

        public AuditLogsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAuditLogsQuery(page, pageSize), cancellationToken);
            return Ok(result);
        }
    }
}