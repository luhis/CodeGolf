namespace CodeGolf.Persistence.Setup
{
    using CodeGolf.Domain;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class SetupHole
    {
        public static void Setup(EntityTypeBuilder<HoleInstance> entity)
        {
            entity.HasKey(e => e.HoleId);
        }
    }
}
