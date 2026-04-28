using System.Text.Json;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BudgetFlow.Infrastructure.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly CurrencySettings _settings;
        private readonly ILogger<CurrencyService> _logger;
        

        // I made a simple cache in memory to not calling api in every request
        private readonly Dictionary<string, (decimal Rate, DateTime CachedAt)> _cache = new();
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

        public CurrencyService(HttpClient httpClient, IOptions<CurrencySettings> settings, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency = "USD")
        {
            fromCurrency = fromCurrency.ToUpper();
            toCurrency = toCurrency.ToUpper();

            // If it's the same currency return 1
            if (fromCurrency == toCurrency)
                return 1m;

            var cacheKey = $"{fromCurrency}_{toCurrency}";

            // if the rate is in the cache and valid return it
            if(_cache.TryGetValue(cacheKey, out var cached))
                if(DateTime.UtcNow - cached.CachedAt < _cacheDuration)
                    return cached.Rate;

            try
            {
                // Connect to the API
                var url = $"{_settings.BaseUrl}/latest?from={fromCurrency}&to={toCurrency}";
                var response = await _httpClient.GetStringAsync(url);

                // Parse the json response
                var json = JsonDocument.Parse(response);
                var rate = json.RootElement
                    .GetProperty("rates")
                    .GetProperty(toCurrency)
                    .GetDecimal();

                // Save it in the cache
                _cache[cacheKey] = (rate, DateTime.UtcNow);
                return rate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get exchange rate for {From} to {To}", fromCurrency, toCurrency);

                // If the API failed return 1 as a fallback to not stop the app
                return 1m;
            }
        }

        public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency = "USD")
        {
            var rate = await GetExchangeRateAsync(fromCurrency, toCurrency);
            return Math.Round(amount * rate, 2);
        }
    }
}