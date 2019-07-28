namespace CodeGolf.Service
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;

    public class BestAttemptsService : IBestAttemptsService
    {
        private readonly IAttemptRepository attemptRepository;

        public BestAttemptsService(IAttemptRepository attemptRepository)
        {
            this.attemptRepository = attemptRepository;
        }

        async Task<IOrderedEnumerable<Attempt>> IBestAttemptsService.GetBestAttempts(
            Guid holeId,
            CancellationToken cancellationToken)
        {
            var attempts = await this.attemptRepository.GetAttempts(holeId, cancellationToken);
            return attempts.OrderBy(a => a.Score).GroupBy(a => a.UserId).Select(a => a.OrderBy(x => x.Score).ThenBy(x => x.TimeStamp).First()).OrderBy(x => x.Score).ThenBy(a => a.TimeStamp);
        }
    }
}