namespace CodeGolf.Integration.Test.Pages
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using Xunit;

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