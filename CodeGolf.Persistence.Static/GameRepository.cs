using System;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;

namespace CodeGolf.Persistence.Static
{
    using System.Collections.Generic;
    using System.Linq;

    using CodeGolf.Domain.ChallengeInterfaces;

    using Optional;
    using Optional.Collections;

    public class GameRepository : IGameRepository
    {
        private static readonly Game Game = new Game(
            new[]
                {
                    new Hole(
                        Guid.Parse("5ccbb74c-1972-47cd-9c5c-f2f512ad95e5"),
                        Challenges.HelloWorld,
                        TimeSpan.FromMinutes(5)),
                    new Hole(
                        Guid.Parse("d44ee76a-ccde-4006-aa83-86578296a886"),
                        Challenges.AlienSpeak,
                        TimeSpan.FromMinutes(5)),
                });

        Game IGameRepository.GetGame()
        {
            return Game;
        }

        Option<Hole> IGameRepository.GetById(Guid id)
        {
            return Game.Holes.SingleOrNone(b => b.HoleId.Equals(id));
        }

        Option<Hole> IGameRepository.GetAfter(Guid id)
        {
            return GetAfter(Game.Holes, item => item.HoleId.Equals(id));
        }

        private static Option<T> GetAfter<T>(IReadOnlyList<T> list, Func<T, bool> equals) =>
            list.SkipWhile(b => !equals(b)).SkipWhile(equals).SingleOrNone();
    }

    public class ChallengeRepository : IChallengeRepository
    {
        private readonly static IReadOnlyList<IChallengeSet> Cs = new IChallengeSet[] { Challenges.HelloWorld, Challenges.AlienSpeak, Challenges.FizzBuzz, Challenges.Calculator };

        public Option<IChallengeSet> GetById(Guid id)
        {
            return Cs.SingleOrNone(a => a.Id == id);
        }
    }

    public interface IChallengeRepository
    { Option<IChallengeSet> GetById(Guid id);
    }
}
