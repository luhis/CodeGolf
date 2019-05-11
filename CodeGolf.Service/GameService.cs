using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain.Repositories;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public class GameService : IGameService
    {
        private readonly Game game = new Game(new[]
        {
            new GameSlot(Challenges.HelloWorld, TimeSpan.FromMinutes(5), new List<Attempt>()),
            new GameSlot(Challenges.AlienSpeak, TimeSpan.FromMinutes(5), new List<Attempt>()),
        });

        private readonly ICodeGolfService codeGolfService;
        private readonly IAttemptRepository attemptRepository;

        public GameService(ICodeGolfService codeGolfService, IAttemptRepository attemptRepository)
        {
            this.codeGolfService = codeGolfService;
            this.attemptRepository = attemptRepository;
        }

        Option<Game> IGameService.GetGame()
        {
            return Option.Some<Game>(this.game);
        }

        Option<GameSlot> IGameService.GetCurrent()
        {
            return Option.Some<GameSlot>(this.game.Slots.First());
        }

        public async Task<Option<int, ErrorSet>> Attempt(Guid userId, string code, ChallengeSet<string> challengeSet)
        {
            var res = await this.codeGolfService.Score(code, challengeSet);
            res.Map(success => this.attemptRepository.AddAttempt(new Domain.Attempt(userId, code, success)));
            return res;
        }
    }
}