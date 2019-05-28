using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;
using Optional;

namespace CodeGolf.Persistence
{
    public class HoleRepository : IRoundRepository
    {
        private static readonly List<HoleInstance> Rounds = new List<HoleInstance>();

        Task<Option<HoleInstance>> IRoundRepository.GetCurrentHole()
        {
            if (Rounds.Any())
            {
                return Task.FromResult(Option.Some(Rounds.Last()));
            }
            else
            {
                return Task.FromResult(Option.None<HoleInstance>());
            }
        }

        Task IRoundRepository.AddHole(HoleInstance hole)
        {
            Rounds.Add(hole);
            return Task.CompletedTask;
        }

        Task IRoundRepository.ClearAll()
        {
            Rounds.Clear();
            return Task.CompletedTask;
        }
    }
}