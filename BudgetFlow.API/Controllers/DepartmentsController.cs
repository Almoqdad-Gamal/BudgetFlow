using BudgetFlow.Application.Features.Departments.Commands.CreateDepartment;
using BudgetFlow.Application.Features.Departments.Commands.DeleteDepartment;
using BudgetFlow.Application.Features.Departments.Commands.UpdateDepartment;
using BudgetFlow.Application.Features.Departments.Queries.GetDepartments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly ISender _sender;
        public DepartmentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetDepartments), result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments( CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetDepartmentsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPut("id:guid")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command with {Id = id}, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("id:guid")]
        public async Task<IActionResult> DeleteDepartment(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteDepartmentCommand(id), cancellationToken);
            return NoContent();
        }
    }
}