namespace CodeGolf.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Optional;

    public interface IHoleRepository
    {
        Task<Option<Hole>> GetCurrentHole(CancellationToken cancellationToken);

        Task EndHole(Guid holeId, DateTime closeTime, CancellationToken cancellationToken);

        Task AddHole(Hole hole, CancellationToken cancellationToken);

        Task ClearAll(CancellationToken cancellationToken);

        Task Update(Hole hole, CancellationToken cancellationToken);

        Task<IReadOnlyList<Hole>> GetGameHoles(Guid gameId, CancellationToken cancellationToken);
    }
}
