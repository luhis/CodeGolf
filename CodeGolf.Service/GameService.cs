using System;
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
        private readonly ICodeGolfService codeGolfService;
        private readonly IAttemptRepository attemptRepository;
        private readonly IGameRepository gameRepository;

        public GameService(ICodeGolfService codeGolfService, IAttemptRepository attemptRepository, IGameRepository gameRepository)
        {
            this.codeGolfService = codeGolfService;
            this.attemptRepository = attemptRepository;
            this.gameRepository = gameRepository;
        }

        Option<Game> IGameService.GetGame()
        {
            return Option.Some(this.gameRepository.GetGame());
        }

        async Task<Option<Round>> IGameService.GetCurrentRound()
        {
            var curr = this.gameRepository.GetGame().Rounds.First();
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