namespace BudgetFlow.Domain.Enums
{
    public enum ExpenseStatus
    {
        Pending = 0,
        ApprovedByManager = 1,
        RejectedByManager = 2,
        ApprovedByFinance = 3,
        RejectedByFinance = 4
    }
}