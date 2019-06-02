using CodeGolf.Domain.Repositories;
using CodeGolf.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Persistence
{
    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddScoped<IAttemptRepository, AttemptRepository>();
            services.AddScoped<IHoleRepository, HoleRepository>();
            services.AddDbContext<CodeGolfContext>();
        }
    }
}