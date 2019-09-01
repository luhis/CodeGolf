namespace CodeGolf.Persistence
{
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Persistence.Repositories;
    using Microsoft.Extensions.DependencyInjection;

    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddScoped<IAttemptRepository, AttemptRepository>();
            services.AddScoped<IHoleRepository, HoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddDbContext<CodeGolfContext>();
        }
    }
}
