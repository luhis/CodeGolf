namespace CodeGolf.CollectableAssembly
{
    using CodeGolf.ServiceInterfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddScoped<ILoadAssembly, LoadAssembly>();
        }
    }
}
