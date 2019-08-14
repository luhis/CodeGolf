namespace CodeGolf.Persistence.Repositories
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Optional;

    public class UserRepository : IUserRepository
    {
        private readonly CodeGolfContext context;

        public UserRepository(CodeGolfContext context)
        {
            this.context = context;
        }

        Task<Option<User>> IUserRepository.GetByUserName(string userName, CancellationToken cancellationToken)
        {
            return this.context.Users.SingleOrNone(a => a.LoginName == userName, cancellationToken);
        }

        Task<Option<User>> IUserRepository.GetByUserId(int userId, CancellationToken cancellationToken)
        {
            return this.context.Users.SingleOrNone(a => a.UserId == userId, cancellationToken);
        }

        async Task IUserRepository.AddOrUpdate(User user, CancellationToken cancellationToken)
        {
            var existing = await this.context.Users.Where(a => a.UserId == user.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            if (existing == null)
            {
                this.context.Users.Add(user);
            }
            else
            {
                this.context.Users.Update(new User(user.UserId, user.LoginName, user.RealName, user.AvatarUri));
            }

            await this.context.SaveChangesAsync(cancellationToken);
        }

        Task IUserRepository.ClearAll()
        {
            this.context.Users.RemoveRange(this.context.Users);
            return this.context.SaveChangesAsync();
        }
    }
}