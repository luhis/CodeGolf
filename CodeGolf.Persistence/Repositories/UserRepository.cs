using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace CodeGolf.Persistence.Repositories
{
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
                // todo this is busted
                this.context.Users.Update(new User(user.UserId, user.LoginName, user.AvatarUri));
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