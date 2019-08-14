namespace CodeGolf.Persistence.Setup
{
    using CodeGolf.Domain;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class SetupAttempt
    {
        public static void Setup(EntityTypeBuilder<Attempt> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HoleId).IsRequired();
            entity.Property(e => e.Score).IsRequired();
            entity.Property(e => e.TimeStamp).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Code).IsRequired();
            entity.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);
            entity.HasOne<HoleInstance>().WithMany().HasForeignKey(p => p.HoleId);
        }
    }
}