using System;
using System.Collections.Generic;
using System.Linq;
using CodeGolf.Dtos;

namespace CodeGolf
{
    public class GameService : IGameService
    {
        private readonly Game game = new Game(new[]
        {
            new GameSlot(Challenges.HelloWorld, TimeSpan.FromMinutes(5), new List<Attempt>()),
            new GameSlot(Challenges.AlienSpeak, TimeSpan.FromMinutes(5), new List<Attempt>()),
        });

        Game IGameService.GetGame()
        {
            return this.game;
        }

        GameSlot IGameService.GetCurrent()
        {
            return this.game.Slots.First();
        }
    }
}