using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CodeGolf.Web;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace CodeGolf.Unit.Test.Pages
{
    public class HomePageShould
    {
        private readonly HttpClient client;

        public HomePageShould()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());
            this.client = server.CreateClient();
        }

        [Fact]
        public async Task GetHomePage()
        {
            var response = await this.client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task NotFindIncorrectPage()
        {
            var response = await this.client.GetAsync("/IMNOTHERE");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}