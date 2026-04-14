using BudgetFlow.Application.Features.Auth.Commands.Login;
using BudgetFlow.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}