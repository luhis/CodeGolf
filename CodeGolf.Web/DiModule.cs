using System;
using System.Net;
using CodeGolf.Persistence;
using CodeGolf.Service;
using CodeGolf.ServiceInterfaces;
using CodeGolf.Web.Attributes;
using CodeGolf.Web.Hubs;
using CodeGolf.Web.Tooling;
using CodeGolf.Web.WebServices;
using JKang.IpcServiceFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Web
{
    public static class DiModule
    {
        private static DbContextOptions<CodeGolfContext> GetDbOptions(IServiceProvider a) => new DbContextOptionsBuilder<CodeGolfContext>()
            .UseSqlite(a.GetService<IConfiguration>().GetSection("DbPath").Get<string>()).Options;

        public static void Add(IServiceCollection collection)
        {
            collection.AddSingleton<DbContextOptions<CodeGolfContext>>(GetDbOptions);
            collection.AddSingleton<IIdentityTools, IdentityTools>();
            collection.AddSingleton<IGetIp, GetIp>();
            collection.AddSingleton<ISignalRNotifier, SignalRNotifier>();
            collection.AddSingleton<IGameAdminChecker, GameAdminChecker>();
            collection.AddSingleton<RecaptchaAttribute>();
            collection.AddSingleton<GameAdminAuthAttribute>();
            collection.AddSingleton<IpcServiceClient<IExecutionService>>(a => new IpcServiceClientBuilder<IExecutionService>()
                .UseTcp(IPAddress.Loopback, SharedSettings.PortNumber).Build());
        }
    }
}