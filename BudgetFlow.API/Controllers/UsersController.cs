using BudgetFlow.Application.Features.Users.Commands.AddUser;
using BudgetFlow.Application.Features.Users.Commands.DeactivateUser;
using BudgetFlow.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;
        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> AddUser([FromBody] AddUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUsers), result);
        }

        [HttpGet]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetUsersQuery(), cancellationToken);
            return Ok(result);
        }
        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateUserCommand(id), cancellationToken);
            return Ok(result);
        }
    }
}