namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using Xunit;

    public class CodeControllerShould : IClassFixture<CustomWebApplicationFactory<CodeGolf.Web.Startup>>
    {
        private readonly HttpClient client;

        public CodeControllerShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.client = fixture.CreateClient();
        }

        [Fact]
        public async Task GetCodeTemplate()
        {
            var response = await this.client.PostAsync("/api/code/preview", null);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetDebugCode()
        {
            var response = await this.client.PostAsync("/api/code/debug", null);
            response.EnsureSuccessStatusCode();
        }
    }
}