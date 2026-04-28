using System.Text.Json;
using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Features.Expenses.Services;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Expenses.Commands.ReviewExpense
{
    public class ReviewExpenseCommandHandler : IRequestHandler<ReviewExpenseCommand, ReviewExpenseResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly BudgetAlertService _budgetAlertService;
        public ReviewExpenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IAuditService auditService, BudgetAlertService budgetAlertService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _budgetAlertService = budgetAlertService;
        }

        public async Task<ReviewExpenseResponse> Handle(ReviewExpenseCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            var reviewerId = _currentUserService.UserId;
            var role = _currentUserService.Role;

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == request.ExpenseId && e.TenantId == tenantId, cancellationToken);

            if(expense is null)
                throw new NotFoundException("Expense", request.ExpenseId);

            if(role == nameof(UserRole.Manager) && expense.Status != ExpenseStatus.Pending)
                throw new ForbiddenException("This expense is not pending manager review.");

            if (role == nameof(UserRole.Finance) && expense.Status != ExpenseStatus.ApprovedByManager)
                throw new ForbiddenException("This expense must be approved by a manager first.");

            var previousStatus = expense.Status;

            // Determining the new status based on the role and decision
            expense.Status = (role, request.IsApproved) switch
            {
                ("Manager", true) => ExpenseStatus.ApprovedByManager,
                ("Manager", false) => ExpenseStatus.RejectedByManager,
                ("Finance", true) => ExpenseStatus.ApprovedByFinance,
                ("Finance", false) => ExpenseStatus.RejectedByFinance,
                _ => throw new ForbiddenException("Only Managers and Finance can review expense.")
            };

            expense.ReviewedByUserId = reviewerId;
            expense.ReviewedAt = DateTime.UtcNow;

            if(!request.IsApproved)
                expense.RejectionReason = request.RejectedReason;
            
            // If the Finance approves, update the BudgetPeriod
            if (role == nameof(UserRole.Finance) && request.IsApproved)
                await UpdateBudgetPeriodAsync(expense, cancellationToken);

            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync(
                action: request.IsApproved ? "ApproveExpense" : "RejectExpense",
                entityName: "Expense",
                oldValues: JsonSerializer.Serialize(new {Status = previousStatus.ToString() }),
                newValues: JsonSerializer.Serialize(new {Status = expense.Status.ToString() }),
                cancellationToken: cancellationToken
            );

            if (role == nameof(UserRole.Finance) && request.IsApproved)
                await _budgetAlertService.CheckAndSendAlertAsync(expense, cancellationToken);

            return new ReviewExpenseResponse
            (
                expense.Id,
                expense.Status.ToString()
            );
        }

        private async Task UpdateBudgetPeriodAsync(Domain.Entities.Expense expense, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var budgetPeriod = await _context.BudgetPeriods
                .FirstOrDefaultAsync(b =>
                    b.DepartmentId == expense.DepartmentId &&
                    b.TenantId == expense.TenantId &&
                    b.Month == now.Month &&
                    b.Year == now.Year,
                    cancellationToken);

            if(budgetPeriod is not null)
            {
                budgetPeriod.SpentAmount += expense.AmountInBaseCurrency;
                _context.BudgetPeriods.Update(budgetPeriod);
            }
        }
    }
}