namespace CodeGolf.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;

    using Optional;
    using Optional.Collections;

    public class GameRepository : IGameRepository
    {
        private static readonly IReadOnlyList<Game> Game = new[] { new Game("test") };

        IReadOnlyList<Game> IGameRepository.GetMyGames(int userId, CancellationToken cancellationToken)
        {
            return Game;
        }

        Game IGameRepository.GetGame(Guid gameId, CancellationToken cancellationToken)
        {
            return Game.First();
        }

        Option<Game> IGameRepository.GetByAccessKey(string accessKey, CancellationToken cancellationToken)
        {
            return Game.SingleOrNone(a => a.AccessKey == accessKey);
        }

        Option<Hole> IGameRepository.GetByHoleId(Guid holeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            ////  return Game.Holes.SingleOrNone(b => b.HoleId.Equals(holeId));
        }

        Option<Hole> IGameRepository.GetAfter(Guid holeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //// return GetAfter(Game.Holes, item => item.HoleId.Equals(holeId));
        }

        private static Option<T> GetAfter<T>(IReadOnlyList<T> list, Func<T, bool> equals) =>
            list.SkipWhile(b => !equals(b)).SkipWhile(equals).SingleOrNone();
    }
}
