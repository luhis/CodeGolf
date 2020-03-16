namespace CodeGolf.Integration.Test.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using CodeGolf.Integration.Test.Tooling;
    using CodeGolf.Persistence;
    using CodeGolf.Recaptcha;
    using CodeGolf.Web.WebServices;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private static readonly IReadOnlyList<Type> ToRemove = new[]
        {
            typeof(DbContextOptions<CodeGolfContext>),
        };

        public HttpClient GetUnAuthorisedClient()
            => this.CreateClient(new WebApplicationFactoryClientOptions() { HandleCookies = false, AllowAutoRedirect = false });

        public HttpClient GetAuthorisedNonAdmin()
        {
            var c = this.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services
                            .AddAuthentication(NonAdminAuthHandler.TestAuthSchemeName)
                            .AddScheme<AuthenticationSchemeOptions, NonAdminAuthHandler>(NonAdminAuthHandler.TestAuthSchemeName, options => { });
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(NonAdminAuthHandler.TestAuthSchemeName);
            return c;
        }

        public HttpClient GetAuthorisedAdmin()
        {
            var c = this.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services
                            .AddAuthentication(AdminAuthHandler.TestAuthSchemeName)
                            .AddScheme<AuthenticationSchemeOptions, AdminAuthHandler>(AdminAuthHandler.TestAuthSchemeName, options => { });
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AdminAuthHandler.TestAuthSchemeName);
            return c;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Where(
                    d => ToRemove.Contains(d.ServiceType)).ToList();

                foreach (var d in descriptor)
                {
                    services.Remove(d);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<CodeGolfContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
                services.AddSingleton<IGetIp, GetMockIp>();
                services.AddSingleton<IRecaptchaVerifier, MockRecaptchaVerifier>();

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<CodeGolfContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        DbInitialiser.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(
                            ex,
                            "An error occurred seeding the database with test messages. Error: {Message}",
                            ex.Message);
                    }
                }
            });

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("GitHub:ClientId", "aaa"),
                    new KeyValuePair<string, string>("GitHub:ClientSecret", "aaa"),
                    new KeyValuePair<string, string>("DbPath", "Data Source=codeGolf.db"),
                    new KeyValuePair<string, string>("Execution:UseRemoteService", "false"),
                }));
        }
    }
}
