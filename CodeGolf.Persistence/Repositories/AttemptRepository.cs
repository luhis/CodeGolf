using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;

namespace CodeGolf.Persistence.Repositories
{
    public class AttemptRepository : IAttemptRepository
    {
        private readonly CodeGolfContext context;

        public AttemptRepository(CodeGolfContext context)
        {
            this.context = context;
        }

        Task IAttemptRepository.AddAttempt(Attempt attempt)
        {
            this.context.Attempts.Add(attempt);
            return this.context.SaveChangesAsync();
        }

        Task<IReadOnlyList<Attempt>> IAttemptRepository.GetAttempts(Guid holeId, CancellationToken cancellationToken)
        {
            return this.context.Attempts.Where(a => a.HoleId == holeId).ToReadOnlyAsync(cancellationToken);
        }

        Task IAttemptRepository.ClearAll()
        {
            this.context.Attempts.RemoveRange(this.context.Attempts);
            return this.context.SaveChangesAsync();
        }
    }
}
