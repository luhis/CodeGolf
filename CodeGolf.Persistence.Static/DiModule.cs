namespace CodeGolf.Persistence.Static
{
    using CodeGolf.Domain.Repositories;
    using Microsoft.Extensions.DependencyInjection;

    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IChallengeRepository, ChallengeRepository>();
        }
    }
}
