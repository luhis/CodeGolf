using System;
using System.Net.Http;
using CodeGolf.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace CodeGolf.Integration.Test.Fixtures
{
    public class ClientFixture : IDisposable
    {
        public ClientFixture()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>().UseSetting("GitHub:ClientId", "aaa").UseSetting("GitHub:ClientSecret", "aaa").UseSetting("DbPath", "Data Source=codeGolf.db"));
                ////.Configure(a => a.UseMiddleware<AuthenticatedTestRequestMiddleware>())
                ////.ConfigureServices(a => a.AddMvc().AddApplicationPart(typeof(DemoModel).Assembly)));
            this.Client = server.CreateClient();
        }

        public void Dispose()
        {
        }

        public HttpClient Client { get; }
    }
}