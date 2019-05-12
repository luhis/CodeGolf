using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeGolf.Domain.Repositories
{
    public interface IAttemptRepository
    {
        Task AddAttempt(Attempt attempt);

        Task<IReadOnlyList<Attempt>> GetAttempts(Guid roundId);
    }
}
