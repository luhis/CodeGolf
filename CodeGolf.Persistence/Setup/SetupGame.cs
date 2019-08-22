namespace CodeGolf.Persistence.Setup
{
    using CodeGolf.Domain;

    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class SetupGame
    {
        public static void Setup(EntityTypeBuilder<Game> entity)
        {
             entity.HasKey(e => e.Id);
             entity.Property(e => e.AccessKey).IsRequired();
        }
    }
}
