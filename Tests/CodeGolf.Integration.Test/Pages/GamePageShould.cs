namespace CodeGolf.Integration.Test.Pages
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using FluentAssertions;
    using Xunit;

    public class GamePageShould : IClassFixture<CustomWebApplicationFactory<CodeGolf.Web.Startup>>
    {
        private readonly HttpClient client;

        public GamePageShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.client = fixture.CreateClient();
        }

        [Fact(Skip = "Need to fix auth mock")]
        public async Task GetGame()
        {
            var response = await this.client.GetAsync("/game");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RequireAuth()
        {
            var response = await this.client.GetAsync("/game");
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }
    }
}