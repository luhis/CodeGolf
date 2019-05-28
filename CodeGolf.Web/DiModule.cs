using CodeGolf.Web.Attributes;
using CodeGolf.Web.Tooling;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Web
{
    public static class DiModule
    {
        public static void Add(IServiceCollection collection)
        {
            collection.AddSingleton<IIdentityTools, IdentityTools>();
            collection.AddSingleton<IGetIp, GetIp>();
            collection.AddSingleton<RecaptchaAttribute>();
            collection.AddSingleton<GameAdminAuthAttribute>();
        }
    }
}