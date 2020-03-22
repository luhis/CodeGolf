﻿namespace CodeGolf.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Persistence.Static;

    using Optional;
    using Optional.Collections;

    public class GameRepository : IGameRepository
    {
        private static readonly Game Game = new Game(
            new[]
                {
                    new Hole(
                        Guid.Parse("5ccbb74c-1972-47cd-9c5c-f2f512ad95e5"),
                        Challenges.HelloWorld.Id,
                        TimeSpan.FromMinutes(5),
                        1),
                    new Hole(
                        Guid.Parse("d44ee76a-ccde-4006-aa83-86578296a886"),
                        Challenges.AlienSpeak.Id,
                        TimeSpan.FromMinutes(5),
                        2),
                });

        Game IGameRepository.GetGame(CancellationToken cancellationToken)
        {
            return Game;
        }

        Option<Hole> IGameRepository.GetByHoleId(Guid holeId, CancellationToken cancellationToken)
        {
            return Game.Holes.SingleOrNone(b => b.HoleId.Equals(holeId));
        }

        Option<Hole> IGameRepository.GetAfter(Guid holeId, CancellationToken cancellationToken)
        {
            return GetAfter(Game.Holes, item => item.HoleId.Equals(holeId));
        }

        private static Option<T> GetAfter<T>(IReadOnlyList<T> list, Func<T, bool> equals) =>
            list.SkipWhile(b => !equals(b)).SkipWhile(equals).SingleOrNone();
    }
}
