namespace CodeGolf.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Optional;

    public interface IAttemptRepository
    {
        Task AddAttempt(Attempt attempt, CancellationToken cancellationToken);

        Task<IReadOnlyList<Attempt>> GetAttempts(Guid holeId, CancellationToken cancellationToken);

        Task ClearAll(CancellationToken cancellationToken);

        Task<Option<Attempt>> GetAttempt(Guid attemptId, CancellationToken cancellationToken);
    }
}
