namespace CodeGolf.Integration.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CodeGolf.Integration.Test.Fixtures;
    using CodeGolf.Web;
    using CodeGolf.Web.Models;
    using FluentAssertions;
    using Xunit;

    public class ChallengeControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public ChallengeControllerShould(CustomWebApplicationFactory<CodeGolf.Web.Startup> fixture)
        {
            this.client = fixture.CreateClient();
        }

        [Fact]
        public async Task GetDemoChallenge()
        {
            var response = await this.client.GetAsync("/api/challenge/DemoChallenge/");
            response.EnsureSuccessStatusCode();
            ////var body = await response.Content.ReadAsStringAsync();
            ////var a = JsonSerializer.Deserialize<ChallengeSetDto>(body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true,  });
            ////a.Should().BeEquivalentTo(new ChallengeSetDto(
            ////    Guid.Parse("08d16a48-4dbb-4f93-9c69-41ff0ab5a417"),
            ////    "This isn't rocket science",
            ////    @"Write a program or function that takes in a single-line string. You can assume it only contains printable ASCII. Print or return a string of an ASCII art rocket such as.",
            ////    "string",
            ////    new List<ParamsDescriptionDto>(),
            ////    new List<ChallengeDto>()));
        }

        ////[Fact]
        ////public async Task SubmitDemo()
        ////{
        ////    var response = await this.client.PostAsync("/api/challenge/SubmitDemo", new StringContent("\"code\"", Encoding.UTF8, "application/json"));
        ////    var b = await response.Content.ReadAsStringAsync();
        ////    response.EnsureSuccessStatusCode();
        ////    ////var body = await response.Content.ReadAsStringAsync();
        ////    ////var a = JsonSerializer.Deserialize<ChallengeSetDto>(body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true,  });
        ////    ////a.Should().BeEquivalentTo(new ChallengeSetDto(
        ////    ////    Guid.Parse("08d16a48-4dbb-4f93-9c69-41ff0ab5a417"),
        ////    ////    "This isn't rocket science",
        ////    ////    @"Write a program or function that takes in a single-line string. You can assume it only contains printable ASCII. Print or return a string of an ASCII art rocket such as.",
        ////    ////    "string",
        ////    ////    new List<ParamsDescriptionDto>(),
        ////    ////    new List<ChallengeDto>()));
        ////}
    }
}
