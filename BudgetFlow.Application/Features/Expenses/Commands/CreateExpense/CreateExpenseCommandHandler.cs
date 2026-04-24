using System.Text.Json;
using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, CreateExpenseResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrencyService _currencyService;
        private readonly IAuditService _auditService;

        public CreateExpenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, ICurrencyService currencyService, IAuditService auditService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _currencyService = currencyService;
            _auditService = auditService;
        }

        public async Task<CreateExpenseResult> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            var userId = _currentUserService.UserId;

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.TenantId == tenantId, cancellationToken);

            if(department is null)
                throw new NotFoundException("Department", request.DepartmentId);

            var amountInBase = await _currencyService.ConvertAsync(
                request.Amount,
                request.Currency,
                "USD"
            );

            var expense = new Expense
            {
                Title = request.Title,
                Amount = request.Amount,
                Currency = request.Currency.ToUpper(),
                AmountInBaseCurrency = amountInBase,
                Notes = request.Notes,
                Status = ExpenseStatus.Pending,
                CreatedByUserId = userId,
                TenantId = tenantId,
                DepartmentId = request.DepartmentId
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync(
                action: "CreateExpense",
                entityName: "Expense",
                newValues: JsonSerializer.Serialize(new
                {
                    expense.Id,
                    expense.Title,
                    expense.Amount,
                    expense.Currency
                }),
                cancellationToken: cancellationToken
            );

            return new CreateExpenseResult
            (
                expense.Id,
                expense.Title,
                expense.Amount,
                expense.Status.ToString()
            );
        }
    }
}