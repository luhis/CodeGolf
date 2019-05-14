using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;

namespace CodeGolf.Persistence
{
    public class AttemptRepository : IAttemptRepository
    {
        private static readonly List<Attempt> Attempts = new List<Attempt>();

        Task IAttemptRepository.AddAttempt(Attempt attempt)
        {
            Attempts.Add(attempt);
            return Task.CompletedTask;
        }

        Task<IReadOnlyList<Attempt>> IAttemptRepository.GetAttempts(Guid roundId)
        {
            return Task.FromResult<IReadOnlyList<Attempt>>(Attempts.Where(a => a.RoundId == roundId).ToList());
        }
    }
}
