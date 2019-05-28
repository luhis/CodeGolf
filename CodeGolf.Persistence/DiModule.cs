using CodeGolf.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Persistence
{
    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddScoped<IAttemptRepository, AttemptRepository>();
            services.AddScoped<IRoundRepository, HoleRepository>();
        }
    }
}