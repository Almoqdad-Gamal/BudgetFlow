namespace BudgetFlow.Application.Common.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "You don't have permission to perform this action.") : base(message) {}
    }
}