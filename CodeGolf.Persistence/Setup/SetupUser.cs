namespace CodeGolf.Persistence.Setup
{
    using CodeGolf.Domain;

    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class SetupUser
    {
        public static void Setup(EntityTypeBuilder<User> entity)
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.LoginName).IsUnique();
            entity.Property(e => e.LoginName).IsRequired();
            entity.Property(e => e.RealName).IsRequired();
            entity.Property(e => e.AvatarUri).IsRequired();
        }
    }
}
