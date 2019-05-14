using System.Threading.Tasks;
using Optional;

namespace CodeGolf.Domain.Repositories
{
    public interface IRoundRepository
    {
        Task<Option<RoundInstance>> GetCurrentRound();

        Task AddRound(RoundInstance round);
    }
}