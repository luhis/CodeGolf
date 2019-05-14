using System.Net.Http;
using System.Threading.Tasks;
using CodeGolf.Web;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.TestHost;

namespace CodeGolf.Unit.Test.Pages
{
    public class DemoPageShould
    {
        private readonly TestServer server;
        private readonly HttpClient client;

        public DemoPageShould()
        {
            this.server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            this.client = this.server.CreateClient();
        }

        [Fact(Skip = "Test server not working")]
        public async Task GetDemo()
        {
            var response = await this.client.GetAsync("/demo");
            response.EnsureSuccessStatusCode();
        }
    }
}