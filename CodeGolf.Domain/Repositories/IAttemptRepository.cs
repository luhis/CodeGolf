using System.Threading.Tasks;

namespace CodeGolf.Domain.Repositories
{
    public interface IAttemptRepository
    {
        Task AddAttempt(Attempt attempt);
    }
}
