using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGolf.Domain.Repositories
{
    using Optional;

    public interface IAttemptRepository
    {
        Task AddAttempt(Attempt attempt);

        Task<IReadOnlyList<Attempt>> GetAttempts(Guid holeId, CancellationToken cancellationToken);

        Task ClearAll();

        Task<Option<Attempt>> GetAttempt(Guid attemptId, CancellationToken cancellationToken);
    }
}
