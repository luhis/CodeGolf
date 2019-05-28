using System.Threading.Tasks;
using Optional;

namespace CodeGolf.Domain.Repositories
{
    public interface IRoundRepository
    {
        Task<Option<HoleInstance>> GetCurrentHole();

        Task AddHole(HoleInstance hole);

        Task ClearAll();
    }
}