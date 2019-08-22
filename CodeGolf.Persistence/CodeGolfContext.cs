namespace CodeGolf.Persistence
{
    using CodeGolf.Domain;
    using CodeGolf.Persistence.Setup;
    using Microsoft.EntityFrameworkCore;

    public class CodeGolfContext : DbContext
    {
        public CodeGolfContext(DbContextOptions<CodeGolfContext> options)
            : base(options)
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Attempt> Attempts { get; private set; }

        public DbSet<Hole> Holes { get; private set; }

        public DbSet<User> Users { get; private set; }

        public DbSet<Game> Games { get; private set; }

        public void SeedDatabase()
        {
            if (this.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                this.Database.Migrate();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupHole.Setup(modelBuilder.Entity<Hole>());
            SetupAttempt.Setup(modelBuilder.Entity<Attempt>());
            SetupUser.Setup(modelBuilder.Entity<User>());
            SetupGame.Setup(modelBuilder.Entity<Game>());
        }
    }
}
