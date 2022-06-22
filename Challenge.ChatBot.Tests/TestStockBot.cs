
using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;


namespace Challenge.ChatBot.Test
{
    public class ServiceGetStockTest : IClassFixture<InjectionFixture>
    {
        private readonly IServiceGetStock serviceGetStock;
        public ServiceGetStockTest(InjectionFixture injection)
        {
            serviceGetStock = injection.ServiceProvider.GetService<IServiceGetStock>();
        }

        [Fact]
        public async Task TestStockBotGetStockSuccess()
        {
            var returnStock = await serviceGetStock.GetChannel("aapl.us");
            Assert.NotNull(returnStock);
            Assert.True(!string.IsNullOrEmpty(returnStock.Symbol));
            Assert.True(returnStock?.Symbol?.Contains("AAPL.US"));
        }

        [Fact]
        public async Task TestStockBotGetStockFail()
        {
            var returnStock = await serviceGetStock.GetChannel("teste");
            Assert.True(returnStock is null || 
                string.IsNullOrEmpty(returnStock.Symbol) || 
                !returnStock.Symbol.Contains("AAPL.US"));
        }
    }
}
