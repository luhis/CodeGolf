using System;
using System.Net.Http;
using CodeGolf.Web;
using CodeGolf.Web.Pages;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Unit.Test.Pages
{
    public class ClientFixture : IDisposable
    {
        public ClientFixture()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>().UseSetting("GitHub:ClientId", "aaa").UseSetting("GitHub:ClientSecret", "aaa"));
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