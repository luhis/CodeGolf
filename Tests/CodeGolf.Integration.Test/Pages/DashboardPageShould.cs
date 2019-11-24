namespace CodeGolf.Integration.Test.Pages
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using Xunit;

    public class DashboardPageShould : IClassFixture<CustomWebApplicationFactory<CodeGolf.Web.Startup>>
    {
        private readonly HttpClient client;

        public DashboardPageShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> client)
        {
            this.client = client.CreateClient();
        }

        [Fact]
        public async Task GetDashboard()
        {
            var response = await this.client.GetAsync("/dashboard");
            response.EnsureSuccessStatusCode();
        }
    }
}