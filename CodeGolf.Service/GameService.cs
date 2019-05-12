using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public class GameService : IGameService
    {
        private readonly Game game = new Game(new[]
        {
            new Round(Guid.Parse("5ccbb74c-1972-47cd-9c5c-f2f512ad95e5"), Challenges.HelloWorld,
                TimeSpan.FromMinutes(5), new List<Attempt>()),
            new Round(Guid.Parse("d44ee76a-ccde-4006-aa83-86578296a886"), Challenges.AlienSpeak,
                TimeSpan.FromMinutes(5), new List<Attempt>()),
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
            return Option.Some(this.game);
        }

        async Task<Option<Round>> IGameService.GetCurrent()
        {
            var curr = this.game.Rounds.First();
            return Option.Some(new Round(curr.RoundId, curr.ChallengeSet, curr.Duration,
                await this.attemptRepository.GetAttempts(curr.RoundId)));
        }
        async Task<Option<int, ErrorSet>> IGameService.Attempt(Guid userId, Guid gameSlotId, string code, ChallengeSet<string> challengeSet)
        {
            var res = await this.codeGolfService.Score(code, challengeSet);
            res.Map(success => this.attemptRepository.AddAttempt(new Domain.Attempt(userId, gameSlotId, code, success)));
            return res;
        }
    }
}