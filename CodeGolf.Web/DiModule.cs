using CodeGolf.Web.Tooling;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Web
{
    public static class DiModule
    {
        public static void Add(IServiceCollection collection)
        {
            collection.AddScoped<IIdentityTools, IdentityTools>();
        }
    }
}