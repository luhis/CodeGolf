namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using FluentAssertions;
    using Xunit;

    public class CodeControllerShould : IClassFixture<CustomWebApplicationFactory<CodeGolf.Web.Startup>>
    {
        private readonly HttpClient unAuthorisedClient;

        public CodeControllerShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.unAuthorisedClient = fixture.GetUnAuthorisedClient();
        }

        [Fact]
        public async Task GetCodeTemplate()
        {
            var response = await this.unAuthorisedClient.PostAsync("/api/code/preview?code=console.log", null);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("console.log");
        }

        [Fact]
        public async Task GetDebugCode()
        {
            var response = await this.unAuthorisedClient.PostAsync("/api/code/debug?code=console.log", null);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("console.log");
        }

        [Fact]
        public async Task TryCompile()
        {
            var response = await this.unAuthorisedClient.PostAsJsonAsync(
                "/api/code/tryCompile",
                "console.log");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("Invalid token");
        }
    }
}
