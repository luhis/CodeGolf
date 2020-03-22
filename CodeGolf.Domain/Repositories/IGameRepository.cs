namespace CodeGolf.Domain.Repositories
{
    using System;
    using System.Threading;

    using Optional;

    public interface IGameRepository
    {
        Game GetGame(CancellationToken cancellationToken);

        Option<Hole> GetByHoleId(Guid holeId, CancellationToken cancellationToken);

        Option<Hole> GetAfter(Guid holeId, CancellationToken cancellationToken);
    }
}
