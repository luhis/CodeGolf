namespace CodeGolf.Persistence
{
    using System.Threading.Tasks;
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

        public DbSet<HoleInstance> Holes { get; private set; }

        public DbSet<User> Users { get; private set; }

        public Task SeedDatabase()
        {
            return this.Database.MigrateAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupHole.Setup(modelBuilder.Entity<HoleInstance>());
            SetupAttempt.Setup(modelBuilder.Entity<Attempt>());
            SetupUser.Setup(modelBuilder.Entity<User>());
            //// SetupGame.Setup(modelBuilder.Entity<Game>());
        }
    }
}