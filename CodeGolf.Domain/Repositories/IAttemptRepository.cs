using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGolf.Domain.Repositories
{
    public interface IAttemptRepository
    {
        Task AddAttempt(Attempt attempt);

        Task<IReadOnlyList<Attempt>> GetAttempts(Guid holeId, CancellationToken cancellationToken);

        Task ClearAll();

        Task<int> GetBestScore(Guid holeId, CancellationToken cancellationToken);

        Task<Attempt> GetAttempt(Guid attemptId, CancellationToken cancellationToken);
    }
}
