using BudgetFlow.Application.Features.Expenses.Commands.CreateExpense;
using BudgetFlow.Application.Features.Expenses.Commands.ReviewExpense;
using BudgetFlow.Application.Features.Expenses.Queries.GetExpenses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly ISender _sender;
        public ExpensesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetExpenses), result);
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses([FromQuery] Guid? departmentId, [FromQuery] string? status,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new GetExpensesQuery(departmentId, status, page, pageSize),
                cancellationToken
            );
            return Ok(result);
        }

        [HttpPost("{id:guid}/review")]
        [Authorize(Roles = "Manager,Finance,TenantAdmin")]
        public async Task<IActionResult> ReviewExpense(Guid id, [FromBody] ReviewExpenseCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command with {ExpenseId = id}, cancellationToken);
            return Ok(result);
        }
    }
}