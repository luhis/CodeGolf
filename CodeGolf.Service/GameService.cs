using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;
using CodeGolf.Service.Dtos;
using Optional;
using Optional.Async;

namespace CodeGolf.Service
{
    public class GameService : IGameService
    {
        private readonly ICodeGolfService codeGolfService;
        private readonly IAttemptRepository attemptRepository;
        private readonly IGameRepository gameRepository;
        private readonly IRoundRepository roundRepository;

        public GameService(ICodeGolfService codeGolfService, IAttemptRepository attemptRepository,
            IGameRepository gameRepository, IRoundRepository roundRepository)
        {
            this.codeGolfService = codeGolfService;
            this.attemptRepository = attemptRepository;
            this.gameRepository = gameRepository;
            this.roundRepository = roundRepository;
        }

        Option<Game> IGameService.GetGame()
        {
            return Option.Some(this.gameRepository.GetGame());
        }

        async Task<Option<RoundDto>> IGameService.GetCurrentRound()
        {
            var round = await this.roundRepository.GetCurrentRound();
            return await round.MapAsync(async a =>
            {
                var curr = this.gameRepository.GetGame().Rounds.First(b => b.RoundId.Equals(a.RoundId));
                return new RoundDto(curr.RoundId, curr.ChallengeSet, curr.Duration,
                    await this.attemptRepository.GetAttempts(curr.RoundId));
            });
        }

        async Task<Option<int, ErrorSet>> IGameService.Attempt(Guid userId, Guid gameSlotId, string code,
            ChallengeSet<string> challengeSet)
        {
            var res = await this.codeGolfService.Score(code, challengeSet);
            res.Map(success =>
                this.attemptRepository.AddAttempt(new Domain.Attempt(userId, gameSlotId, code, success)));
            return res;
        }

        private static T GetAfter<T>(IReadOnlyList<T> list, Func<T, bool> equals) =>
            list.SkipWhile(b => !equals(b)).SkipWhile(equals).ElementAt(0);

        async Task IGameService.NextRound()
        {
            var round = await this.roundRepository.GetCurrentRound();
            var next = round.Match(some => GetAfter(this.gameRepository.GetGame().Rounds, item => item.RoundId.Equals(some.RoundId)), 
                () => this.gameRepository.GetGame().Rounds.First());
            await this.roundRepository.AddRound(new RoundInstance(next.RoundId, DateTime.UtcNow,
                DateTime.UtcNow.Add(next.Duration)));
        }
    }
}