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
            var response = await this.client.GetAsync("/code/preview");
            response.EnsureSuccessStatusCode();
        }
    }
}