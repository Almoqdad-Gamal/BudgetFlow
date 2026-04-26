namespace BudgetFlow.Application.Common.Interfaces
{
    public interface IPdfReportService
    {
        byte[] GenerateMonthReport(MonthlyReportData data);
    }

    public class MonthlyReportData
    {
        public string TenantName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalAllocatedBudget { get; set; }
        public decimal TotalSpentAmount { get; set; }
        public decimal TotalRemainingBudget => TotalAllocatedBudget - TotalSpentAmount;
        public List<DepartmentReportData> Departments { get; set; } = new();
    }

    public class DepartmentReportData
    {
        public string DepartmentName { get; set; } = string.Empty;
        public decimal AllocatedBudegt { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingBudget => AllocatedBudegt - SpentAmount;
        public decimal SpentPercentage => AllocatedBudegt == 0 ? 0 : Math.Round((SpentAmount / AllocatedBudegt) * 100, 1);
    }
}