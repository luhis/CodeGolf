namespace CodeGolf.Integration.Test.Pages
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using FluentAssertions;
    using Xunit;

    public class HomePageShould : IClassFixture<ClientFixture>
    {
        private readonly HttpClient client;

        public HomePageShould(ClientFixture fixture)
        {
            this.client = fixture.Client;
        }

        [Fact]
        public async Task GetHomePage()
        {
            var response = await this.client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task NotFindIncorrectPage()
        {
            var response = await this.client.GetAsync("/ImNotThere");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}