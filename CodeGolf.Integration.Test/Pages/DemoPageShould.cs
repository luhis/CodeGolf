using System.Net.Http;
using System.Threading.Tasks;
using CodeGolf.Integration.Test.Pages;
using Xunit;

namespace CodeGolf.Unit.Test.Pages
{
    public class DemoPageShould : IClassFixture<ClientFixture>
    {
        private readonly HttpClient client;

        public DemoPageShould(ClientFixture fixture)
        {
            this.client = fixture.Client;
        }

        [Fact]
        public async Task GetDemo()
        {
            var response = await this.client.GetAsync("/demo");
            response.EnsureSuccessStatusCode();
        }
    }
}