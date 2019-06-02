using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Persistence.Setup;
using Microsoft.EntityFrameworkCore;

namespace CodeGolf.Persistence
{
    public class CodeGolfContext : DbContext
    {
        public CodeGolfContext(DbContextOptions<CodeGolfContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupHole.Setup(modelBuilder.Entity<HoleInstance>());
            SetupAttempt.Setup(modelBuilder.Entity<Attempt>());
        }

        public DbSet<Attempt> Attempts { get; private set; }

        public DbSet<HoleInstance> Hole { get; private set; }

        public Task SeedDatabase()
        {
            return this.Database.MigrateAsync();
        }
    }
}