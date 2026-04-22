namespace BudgetFlow.Application.Common.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> GetExchangeRateAsync (string fromCurrency, string toCurrency = "USD");

        Task<decimal> ConvertAsync (decimal amount, string fromCurrency, string toCurrency = "USD");
    }
}