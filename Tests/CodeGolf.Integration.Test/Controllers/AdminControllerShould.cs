namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using CodeGolf.Integration.Test.Tooling;
    using CodeGolf.Web;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class AdminControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;
        private readonly HttpClient authorisedClient;

        public AdminControllerShould(CustomWebApplicationFactory<Startup> fixture)
        {
            this.client = fixture.CreateClient(new WebApplicationFactoryClientOptions() { HandleCookies = false, AllowAutoRedirect = false });
            this.authorisedClient = fixture.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services
                            .AddAuthentication(TestAuthHandler.TestAuthSchemeName)
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.TestAuthSchemeName, options => { });
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            this.authorisedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.TestAuthSchemeName);
        }

        [Fact]
        public async Task RejectEndHoleFromAnon()
        {
            var response = await this.client.PostAsJsonAsync<object>("/api/admin/endHole", null);
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
            var response = await this.client.PostAsJsonAsync<object>("/api/admin/nextHole", null);
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
