namespace CodeGolf.Integration.Test.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CodeGolf.Integration.Test.Tooling;
    using CodeGolf.Persistence;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<CodeGolfContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<CodeGolfContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

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
