using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Expenses.Services
{
    public class BudgetAlertService
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public BudgetAlertService(IApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task CheckAndSendAlertAsync(Expense expense, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var budgetPeriod = await _context.BudgetPeriods
                .FirstOrDefaultAsync(b => 
                b.DepartmentId == expense.DepartmentId &&
                b.TenantId == expense.TenantId &&
                b.Month == now.Month &&
                b.Year == now.Year,
                cancellationToken);

            if (budgetPeriod is null) return;

            var spentPercentage = budgetPeriod.AllocatedBudget == 0 ? 0
                : (budgetPeriod.SpentAmount / budgetPeriod.AllocatedBudget) * 100;

            if (spentPercentage < 80) return;

            // Getting the tenant admin to send him the alert
            var admin = await _context.Users
                .FirstOrDefaultAsync(u => 
                    u.TenantId == expense.TenantId &&
                    u.Role == UserRole.TenantAdmin &&
                    u.IsActive,
                    cancellationToken);

            if (admin is null) return;

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == expense.DepartmentId, cancellationToken);

            var remaining = budgetPeriod.AllocatedBudget - budgetPeriod.SpentAmount;

            await _emailService.SendAsync(
                toEmail: admin.Email,
                toName: $"{admin.FirstName} {admin.LastName}",
                subject: $"Budget Alert - {department?.Name} reached {spentPercentage:N0}%",
                body: $"""
                    <div style="font-family: sans-serif; max-width: 600px; margin: 0 auto;">
                        <h2 style="color: #DC2626;">Budget Alert</h2>
                        <p>Hi {admin.FirstName},</p>
                        <p>
                            The <strong>{department?.Name}</strong> department has used
                            <strong style="color: #DC2626;">{spentPercentage:N1}%</strong>
                            of its monthly budget.
                        </p>
                        <table style="width: 100%; border-collapse: collapse; margin: 1rem 0;">
                            <tr style="background: #F9FAFB;">
                                <td style="padding: 8px 12px; color: #6B7280;">Allocated</td>
                                <td style="padding: 8px 12px; font-weight: 500;">${budgetPeriod.AllocatedBudget:N2}</td>
                            </tr>
                            <tr>
                                <td style="padding: 8px 12px; color: #6B7280;">Spent</td>
                                <td style="padding: 8px 12px; font-weight: 500; color: #DC2626;">${budgetPeriod.SpentAmount:N2}</td>
                            </tr>
                            <tr style="background: #F9FAFB;">
                                <td style="padding: 8px 12px; color: #6B7280;">Remaining</td>
                                <td style="padding: 8px 12px; font-weight: 500; color: #16A34A;">${remaining:N2}</td>
                            </tr>
                        </table>
                        <p style="color: #6B7280; font-size: 13px;">
                            BudgetFlow — {now:MMMM yyyy}
                        </p>
                    </div>
                    """
            );
        }
    }
}