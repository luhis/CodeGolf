﻿namespace CodeGolf.Integration.Test.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
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
            this.client = fixture.CreateClient(new WebApplicationFactoryClientOptions() { HandleCookies = false, AllowAutoRedirect = false });
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Fact]
        public async Task RejectEndHoleFromAnon()
        {
            var response = await this.client.PostAsJsonAsync<object>("/api/admin/endHole", null);
            var b = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }
    }
}
