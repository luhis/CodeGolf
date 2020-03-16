namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using CodeGolf.Integration.Test.Tooling;
    using CodeGolf.Web;
    using CodeGolf.Web.Models;
    using FluentAssertions;
    using Xunit;

    public class AccessControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient unAuthorisedClient;
        private readonly HttpClient authorisedClient;

        public AccessControllerShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.unAuthorisedClient = fixture.GetUnAuthorisedClient();
            this.authorisedClient = fixture.GetAuthorisedNonAdmin();
        }

        [Fact]
        public async Task GetAccess()
        {
            using var response = await this.unAuthorisedClient.GetAsync("/api/access/getAccess/");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var a = JsonSerializer.Deserialize<AccessDto>(body);
            a.Should().BeEquivalentTo(new AccessDto(false, false, false));
        }

        [Fact]
        public async Task GetAccessLoggedIn()
        {
            var response = await this.authorisedClient.GetAsync("/api/access/getAccess/");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var a = JsonSerializer.Deserialize<AccessDto>(body, JsonOptions.Options);
            a.Should().BeEquivalentTo(new AccessDto(true, false, false));
        }

        [Fact]
        public async Task SignOut()
        {
            var response = await this.unAuthorisedClient.PostAsync("/api/access/signOut/", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
