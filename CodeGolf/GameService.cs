using System;
using CodeGolf.Dtos;

namespace CodeGolf
{
    public class GameService : IGameService
    {
        Game IGameService.GetGame()
        {
            return new Game(new[]
            {
                new GameSlot(Challenges.HelloWorld, TimeSpan.FromMinutes(5)),
                new GameSlot(Challenges.AlienSpeak, TimeSpan.FromMinutes(5)),
            });
        }
    }
}