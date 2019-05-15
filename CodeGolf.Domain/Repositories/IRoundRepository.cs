using System.Threading.Tasks;
using Optional;

namespace CodeGolf.Domain.Repositories
{
    public interface IRoundRepository
    {
        Task<Option<RoundInstance>> GetCurrentHole();

        Task AddRound(RoundInstance round);
    }
}