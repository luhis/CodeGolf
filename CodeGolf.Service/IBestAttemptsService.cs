namespace CodeGolf.Service
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;

    public interface IBestAttemptsService
    {
        Task<IOrderedEnumerable<Attempt>> GetBestAttempts(Guid holeId, CancellationToken cancellationToken);
    }
}
