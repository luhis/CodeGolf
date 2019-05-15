using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;
using Optional;

namespace CodeGolf.Persistence
{
    public class RoundRepository : IRoundRepository
    {
        private static readonly List<RoundInstance> Rounds = new List<RoundInstance>();

        public Task<Option<RoundInstance>> GetCurrentHole()
        {
            if (Rounds.Any())
            {
                return Task.FromResult(Option.Some(Rounds.Last()));
            }
            else
            {
                return Task.FromResult(Option.None<RoundInstance>());
            }
        }

        public Task AddRound(RoundInstance round)
        {
            Rounds.Add(round);
            return Task.CompletedTask;
        }
    }
}