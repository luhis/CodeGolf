namespace CodeGolf.Domain.Repositories
{
    using System.Threading;
    using System.Threading.Tasks;
    using Optional;

    public interface IUserRepository
    {
        Task<Option<User>> GetByUserId(int userId, CancellationToken cancellationToken);

        Task AddOrUpdate(User user, CancellationToken cancellationToken);

        Task ClearAll(CancellationToken cancellationToken);
    }
}
