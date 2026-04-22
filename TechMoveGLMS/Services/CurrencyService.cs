using System.Text.Json;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var response = await _httpClient.GetAsync("https://open.er-api.com/v6/latest/USD");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            if (root.TryGetProperty("rates", out JsonElement ratesElement) &&
                ratesElement.TryGetProperty("ZAR", out JsonElement zarRateElement))
            {
                return zarRateElement.GetDecimal();
            }

            throw new Exception("Unable to retrieve USD to ZAR exchange rate.");
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            return Math.Round(usdAmount * exchangeRate, 2);
        }
    }
}