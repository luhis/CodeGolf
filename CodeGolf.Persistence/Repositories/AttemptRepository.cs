namespace CodeGolf.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using Optional;

    public class AttemptRepository : IAttemptRepository
    {
        private readonly CodeGolfContext context;

        public AttemptRepository(CodeGolfContext context)
        {
            this.context = context;
        }

        Task IAttemptRepository.AddAttempt(Attempt attempt, CancellationToken cancellationToken)
        {
            this.context.Attempts.Add(attempt);
            return this.context.SaveChangesAsync(cancellationToken);
        }

        Task<IReadOnlyList<Attempt>> IAttemptRepository.GetAttempts(Guid holeId, CancellationToken cancellationToken)
        {
            return this.context.Attempts.Where(a => a.HoleId == holeId).ToReadOnlyAsync(cancellationToken);
        }

        Task IAttemptRepository.ClearAll(CancellationToken cancellationToken)
        {
            this.context.Attempts.RemoveRange(this.context.Attempts);
            return this.context.SaveChangesAsync(cancellationToken);
        }

        Task<Option<Attempt>> IAttemptRepository.GetAttempt(Guid attemptId, CancellationToken cancellationToken)
        {
            return this.context.Attempts.SingleOrNone(a => a.Id == attemptId, cancellationToken);
        }
    }
}
