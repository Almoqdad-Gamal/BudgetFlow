namespace BudgetFlow.Application.Common.Interfaces
{
    public interface ISubscriptionService
    {
        // Make checkout session and return the URL that the user will go to it 
        Task<string> CreateCheckoutSessionAsync(Guid tenantId, string tenantEmail);
    }
}