using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetFlow.Infrastructure.Jobs
{
    public class MonthlyReportJob
    {
        private readonly IApplicationDbContext _context;
        private readonly IPdfReportService _pdfService;
        private readonly IEmailService _emailService;
        private readonly ILogger<MonthlyReportJob> _logger;

        public MonthlyReportJob(IApplicationDbContext context, IPdfReportService pdfService, IEmailService emailService, ILogger<MonthlyReportJob> logger)
        {
            _context = context;
            _pdfService = pdfService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            var lastMonth = DateTime.UtcNow.AddMonths(-1);
            var month = lastMonth.Month;
            var year = lastMonth.Year;

            _logger.LogInformation("Starting monthly report job for {Month}/{Year}", month, year);

            // Get all active tenants
            var tenants = await _context.Tenants
                .Where(t => t.IsActive && t.Plan == SubscriptionPlan.Pro)
                .Include(t => t.Users)
                .ToListAsync();

            foreach (var tenant in tenants)
            {
                try
                {
                    // Get budget periods data for this month
                    var budgetPeriods = await _context.BudgetPeriods
                        .Include(b => b.Department)
                        .Where(b => 
                        b.TenantId == tenant.Id && 
                        b.Month == month && 
                        b.Year == year)
                        .ToListAsync();

                    // If there is no data SKIP
                    if(!budgetPeriods.Any())
                        continue;

                    // Prepare data for PDF
                    var reportData = new MonthlyReportData
                    {
                        TenantName = tenant.Name,
                        Month = month,
                        Year = year,
                        TotalAllocatedBudget = budgetPeriods.Sum(b => b.AllocatedBudget),
                        TotalSpentAmount = budgetPeriods.Sum(b => b.SpentAmount),
                        Departments = budgetPeriods.Select(b => new DepartmentReportData
                        {
                            DepartmentName = b.Department.Name,
                            AllocatedBudegt = b.AllocatedBudget,
                            SpentAmount = b.SpentAmount
                        }).ToList()
                    };

                    // Generate PDF
                    var pdfBytes = _pdfService.GenerateMonthReport(reportData);

                    // Send email to tenant admin
                    var adminUser = tenant.Users
                        .FirstOrDefault(u => u.Role == Domain.Enums.UserRole.TenantAdmin);

                    if (adminUser != null)
                    {
                        await _emailService.SendAsync(
                            toEmail: adminUser.Email,
                            toName: $"{adminUser.FirstName} {adminUser.LastName}",
                            subject: $"BudgetFlow - Monthly Report {month}/{year}",
                            body: $"""
                                <h2>Monthly Budget Report</h2>
                                <p>Hi {adminUser.FirstName},</p>
                                <p>Please find attached your monthly budget report for {month}/{year}.</p>
                                <p>Best regards,<br/>BudgetFlow Team</p>
                                """,
                            attachment: pdfBytes,
                            attachmentName: $"BudgetFlow-Report-{month}-{year}.pdf"
                        );

                        _logger.LogInformation("Report sent to {Email} for tenant {TenantName}", adminUser.Email, tenant.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate report for tenant {TenantId}", tenant.Id);
                }
            }
        }
    }
}