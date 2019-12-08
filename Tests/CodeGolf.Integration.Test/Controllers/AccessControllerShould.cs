namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using CodeGolf.Web;
    using CodeGolf.Web.Models;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class AccessControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public AccessControllerShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.client = fixture.CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false });
        }

        [Fact]
        public async Task GetAccess()
        {
            var response = await this.client.GetAsync("/api/access/getAccess/");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var a = JsonSerializer.Deserialize<AccessDto>(body);
            a.Should().BeEquivalentTo(new AccessDto(false, false, false));
        }

        [Fact]
        public async Task SignOut()
        {
            var response = await this.client.PostAsync("/api/access/signOut/", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
