using System.Threading.Tasks;
using Optional;

namespace CodeGolf.Domain.Repositories
{
    using System;

    public interface IHoleRepository
    {
        Task<Option<HoleInstance>> GetCurrentHole();

        Task EndHole(Guid holeId, DateTime closeTime);

        Task AddHole(HoleInstance hole);

        Task ClearAll();
    }
}