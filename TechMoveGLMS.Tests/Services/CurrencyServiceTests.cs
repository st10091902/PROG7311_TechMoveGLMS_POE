using System.Net.Http;
using TechMoveGLMS.Services;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class CurrencyServiceTests
    {
        [Fact]
        public void ConvertUsdToZar_ReturnsRoundedConvertedAmount()
        {
            var httpClient = new HttpClient();
            var service = new CurrencyService(httpClient);

            decimal usdAmount = 100m;
            decimal exchangeRate = 18.75m;

            var result = service.ConvertUsdToZar(usdAmount, exchangeRate);

            Assert.Equal(1875.00m, result);
        }

        [Fact]
        public void ConvertUsdToZar_RoundsToTwoDecimalPlaces()
        {
            var httpClient = new HttpClient();
            var service = new CurrencyService(httpClient);

            decimal usdAmount = 10.555m;
            decimal exchangeRate = 18.333m;

            var result = service.ConvertUsdToZar(usdAmount, exchangeRate);

            Assert.Equal(Math.Round(usdAmount * exchangeRate, 2), result);
        }
    }
}