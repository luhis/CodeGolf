namespace CodeGolf.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Optional;

    public interface IGameRepository
    {
        IReadOnlyList<Game> GetMyGames(int userId, CancellationToken cancellationToken);

        Game GetGame(Guid gameId, CancellationToken cancellationToken);

        Option<Hole> GetByHoleId(Guid holeId, CancellationToken cancellationToken);

        Option<Hole> GetAfter(Guid holeId, CancellationToken cancellationToken);

        Option<Game> GetByAccessKey(string accessKey, CancellationToken cancellationToken);
    }
}
