using System.Net.Http;
using System.Threading.Tasks;
using CodeGolf.Integration.Test.Fixtures;
using Xunit;

namespace CodeGolf.Integration.Test.Controllers
{
    public class CodeControllerShould : IClassFixture<ClientFixture>
    {
        private readonly HttpClient client;

        public CodeControllerShould(ClientFixture fixture)
        {
            this.client = fixture.Client;
        }

        [Fact]
        public async Task GetCodeTemplate()
        {
            var response = await this.client.GetAsync("/api/code/preview");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetDebugCode()
        {
            var response = await this.client.GetAsync("/api/code/debug");
            response.EnsureSuccessStatusCode();
        }
    }
}