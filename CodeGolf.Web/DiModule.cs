using System;
using CodeGolf.Persistence;
using CodeGolf.Web.Attributes;
using CodeGolf.Web.Tooling;
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
            collection.AddSingleton<RecaptchaAttribute>();
            collection.AddSingleton<GameAdminAuthAttribute>();
        }
    }
}