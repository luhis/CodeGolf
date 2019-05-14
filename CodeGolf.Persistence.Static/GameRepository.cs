using System;
using System.Collections.Generic;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;

namespace CodeGolf.Persistence.Static
{
    public class GameRepository : IGameRepository
    {
        private static readonly Game Game = new Game(new[]
        {
            new Round(Guid.Parse("5ccbb74c-1972-47cd-9c5c-f2f512ad95e5"), Challenges.HelloWorld,
                TimeSpan.FromMinutes(5), new List<Attempt>()),
            new Round(Guid.Parse("d44ee76a-ccde-4006-aa83-86578296a886"), Challenges.AlienSpeak,
                TimeSpan.FromMinutes(5), new List<Attempt>()),
        });

        Game IGameRepository.GetGame()
        {
            return Game;
        }
    }
}