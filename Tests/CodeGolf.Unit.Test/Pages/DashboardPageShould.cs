using System.Net.Http;
using System.Threading.Tasks;
using CodeGolf.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace CodeGolf.Unit.Test.Pages
{
    public class DashboardPageShould
    {
        private readonly HttpClient client;

        public DashboardPageShould()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());
            this.client = server.CreateClient();
        }

        [Fact]
        public async Task GetDashboard()
        {
            var response = await this.client.GetAsync("/dashboard");
            response.EnsureSuccessStatusCode();
        }
    }
}