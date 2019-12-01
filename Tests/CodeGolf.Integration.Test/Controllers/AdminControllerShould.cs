namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using CodeGolf.Web;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class AdminControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public AdminControllerShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.client = fixture.CreateClient(new WebApplicationFactoryClientOptions() { HandleCookies = false });
        }

        [Fact(Skip = "Auth needs work")]
        public async Task RejectEndHoleFromAnon()
        {
            var response = await this.client.PostAsync("/api/admin/endhole", null);
            var b = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(403);
        }
    }
}
