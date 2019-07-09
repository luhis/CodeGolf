using CodeGolf.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Persistence.Static
{
    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IChallengeRepository, ChallengeRepository>();
        }
    }
}
