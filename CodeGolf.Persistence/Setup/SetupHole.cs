using CodeGolf.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeGolf.Persistence.Setup
{
    public static class SetupHole
    {
        public static void Setup(EntityTypeBuilder<HoleInstance> entity)
        {
            entity.HasKey(e => e.HoleId);
        }
    }
}
