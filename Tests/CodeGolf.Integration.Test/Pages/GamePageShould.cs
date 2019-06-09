using System.Net.Http;
using System.Threading.Tasks;
using CodeGolf.Integration.Test.Fixtures;
using Xunit;

namespace CodeGolf.Integration.Test.Pages
{
    public class GamePageShould : IClassFixture<ClientFixture>
    {
        private readonly HttpClient client;

        public GamePageShould(ClientFixture fixture)
        {
            this.client = fixture.Client;
        }

        [Fact(Skip = "Need to fix auth mock")]
        public async Task GetGame()
        {
            var response = await this.client.GetAsync("/game");
            response.EnsureSuccessStatusCode();
        }
    }
}