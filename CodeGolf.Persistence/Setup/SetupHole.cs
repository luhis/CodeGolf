namespace CodeGolf.Persistence.Setup
{
    using CodeGolf.Domain;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class SetupHole
    {
        public static void Setup(EntityTypeBuilder<Hole> entity)
        {
            entity.HasKey(e => e.HoleId);
            entity.HasOne<Game>().WithMany().HasForeignKey(p => p.GameId);
        }
    }
}
