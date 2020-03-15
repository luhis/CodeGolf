namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using CodeGolf.Integration.Test.Tooling;
    using CodeGolf.Web;
    using CodeGolf.Web.Models;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class AccessControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;
        private readonly HttpClient authorisedClient;

        public AccessControllerShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.client = fixture.CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false, HandleCookies = false });
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
        public async Task GetAccess()
        {
            var response = await this.client.GetAsync("/api/access/getAccess/");
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
            var response = await this.client.PostAsync("/api/access/signOut/", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
