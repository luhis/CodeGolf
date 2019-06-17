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

    public static class SetupUser
    {
        public static void Setup(EntityTypeBuilder<User> entity)
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.LoginName).IsUnique();
            entity.Property(e => e.LoginName).IsRequired();
            entity.Property(e => e.AvatarUri).IsRequired();
        }
    }
}
