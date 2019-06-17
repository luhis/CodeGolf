using System.Threading;
using System.Threading.Tasks;
using Optional;

namespace CodeGolf.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Option<User>> GetByUserName(string userName, CancellationToken cancellationToken);

        Task AddOrUpdate(User user, CancellationToken cancellationToken);

        Task ClearAll();
    }
}