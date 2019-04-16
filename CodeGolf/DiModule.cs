using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf
{
    public static class DiModule
    {
        public static void Add(IServiceCollection collection)
        {
            collection.AddTransient<ICodeGolfService, CodeGolfService>();
        }
    }
}