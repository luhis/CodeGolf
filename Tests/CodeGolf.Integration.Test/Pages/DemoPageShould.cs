namespace CodeGolf.Integration.Test.Pages
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using Xunit;

    public class DemoPageShould : IClassFixture<CustomWebApplicationFactory<CodeGolf.Web.Startup>>
    {
        private readonly HttpClient client;

        public DemoPageShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.client = fixture.CreateClient();
        }

        [Fact]
        public async Task GetDemo()
        {
            var response = await this.client.GetAsync("/demo");
            var a = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }
    }
}