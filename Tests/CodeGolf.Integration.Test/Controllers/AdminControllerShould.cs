namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using CodeGolf.Web;
    using FluentAssertions;
    using Xunit;

    public class AdminControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient unAuthorisedClient;
        private readonly HttpClient authorisedClient;

        public AdminControllerShould(CustomWebApplicationFactory<Startup> fixture)
        {
            this.unAuthorisedClient = fixture.GetUnAuthorisedClient();
            this.authorisedClient = fixture.GetAuthorisedNonAdmin();
        }

        [Fact]
        public async Task RejectEndHoleFromAnon()
        {
            var response = await this.unAuthorisedClient.PostAsJsonAsync<object>("/api/admin/endHole", null);
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Fact]
        public async Task RejectEndHoleFromNonAdmin()
        {
            var response = await this.authorisedClient.PostAsJsonAsync<object>("/api/admin/endHole", null);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RejectNextHoleFromAnon()
        {
            var response = await this.unAuthorisedClient.PostAsJsonAsync<object>("/api/admin/nextHole", null);
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Fact]
        public async Task RejectNextHoleFromNonAdmin()
        {
            var response = await this.authorisedClient.PostAsJsonAsync<object>("/api/admin/nextHole", null);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
