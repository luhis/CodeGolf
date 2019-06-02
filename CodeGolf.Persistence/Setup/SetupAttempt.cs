using CodeGolf.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeGolf.Persistence.Setup
{
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
        }
    }
}