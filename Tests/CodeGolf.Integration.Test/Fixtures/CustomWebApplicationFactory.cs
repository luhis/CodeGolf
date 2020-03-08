﻿namespace CodeGolf.Integration.Test.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CodeGolf.Integration.Test.Tooling;
    using CodeGolf.Persistence;
    using CodeGolf.Recaptcha;
    using CodeGolf.Web.WebServices;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private static readonly IReadOnlyList<Type> ToRemove = new[]
        {
            typeof(DbContextOptions<CodeGolfContext>),
            typeof(IGetIp),
            typeof(IRecaptchaVerifier),
            typeof(IAuthenticationService),
            typeof(IClaimsTransformation),
            typeof(IAuthenticationHandlerProvider),
            typeof(IAuthenticationSchemeProvider),
            ////typeof(IAuthorizationService),
            ////typeof(IAuthorizationEvaluator),
            typeof(IAuthorizationHandler),
            ////typeof(IAuthorizationHandlerContextFactory),
            ////typeof(IAuthorizationHandlerProvider),
            ////typeof(IAuthorizationPolicyProvider),
        };

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
            }).ConfigureTestServices(
                services =>
                {
                    services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options => { });
                });
        }
    }
}
