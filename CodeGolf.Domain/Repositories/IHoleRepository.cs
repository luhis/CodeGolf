using System.Threading.Tasks;
using Optional;

namespace CodeGolf.Domain.Repositories
{
    public interface IHoleRepository
    {
        Task<Option<HoleInstance>> GetCurrentHole();

        Task AddHole(HoleInstance hole);

        Task ClearAll();
    }
}