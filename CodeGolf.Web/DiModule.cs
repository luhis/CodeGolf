namespace CodeGolf.Web
{
    using System;
    using CodeGolf.ExecutionServer;
    using CodeGolf.Persistence;
    using CodeGolf.Service;
    using CodeGolf.ServiceInterfaces;
    using CodeGolf.Web.Attributes;
    using CodeGolf.Web.Hubs;
    using CodeGolf.Web.Mappers;
    using CodeGolf.Web.Tooling;
    using CodeGolf.Web.WebServices;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class DiModule
    {
        public static void Add(IServiceCollection collection)
        {
            collection.AddSingleton(GetDbOptions);
            collection.AddSingleton<IIdentityTools, IdentityTools>();
            collection.AddSingleton<IGetIp, GetIp>();
            collection.AddSingleton<ISignalRNotifier, SignalRNotifier>();
            collection.AddSingleton<IGameAdminChecker, GameAdminChecker>();
            collection.AddSingleton<RecaptchaAttribute>();
            collection.AddSingleton<GameAdminAuthAttribute>();
            collection.AddSingleton<ChallengeSetMapper>();
            collection.AddSingleton<ExecutionProxy>();
            collection.AddSingleton<ExecutionService>();
            collection.AddSingleton<IExecutionService>(s =>
            {
                var settings = s.GetRequiredService<IOptions<ExecutionSettings>>();
                return settings.Value.UseRemoteService ? (IExecutionService)s.GetRequiredService<ExecutionProxy>() : (IExecutionService)s.GetRequiredService<ExecutionService>();
            });
        }

        private static DbContextOptions<CodeGolfContext> GetDbOptions(IServiceProvider a) => new DbContextOptionsBuilder<CodeGolfContext>()
            .UseSqlite(a.GetService<IConfiguration>().GetSection("DbPath").Get<string>()).Options;
    }
}
