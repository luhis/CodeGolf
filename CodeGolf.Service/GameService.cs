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

        private async Task<IReadOnlyList<Attempt>> GetBestAttempts(Guid holeId)
        {
            var attempts = await this.attemptRepository.GetAttempts(holeId);
            return attempts.OrderByDescending(a => a.Score).GroupBy(a => a.UserId).Select(a => a.First()).OrderByDescending(a => a.Score).ToList();
        }

        async Task<Option<HoleDto>> IGameService.GetCurrentHole()
        {
            var round = await this.roundRepository.GetCurrentHole();
            return await round.MapAsync(async a =>
            {
                var curr = this.gameRepository.GetGame().Holes.First(b => b.HoleId.Equals(a.HoleId));
                return new HoleDto(curr, a.Start, a.End, await this.GetBestAttempts(curr.HoleId));
            });
        }

        async Task<Option<int, ErrorSet>> IGameService.Attempt(Guid userId, Guid holeId, string code,
            ChallengeSet<string> challengeSet)
        {
            var res = await this.codeGolfService.Score(code, challengeSet);
            res.Map(success =>
                this.attemptRepository.AddAttempt(new Domain.Attempt(userId, holeId, code, success)));
            return res;
        }

        private static T GetAfter<T>(IReadOnlyList<T> list, Func<T, bool> equals) =>
            list.SkipWhile(b => !equals(b)).SkipWhile(equals).ElementAt(0);

        async Task IGameService.NextRound()
        {
            var round = await this.roundRepository.GetCurrentHole();
            var next = round.Match(some => GetAfter(this.gameRepository.GetGame().Holes, item => item.HoleId.Equals(some.HoleId)), 
                () => this.gameRepository.GetGame().Holes.First());
            await this.roundRepository.AddRound(new RoundInstance(next.HoleId, DateTime.UtcNow,
                DateTime.UtcNow.Add(next.Duration)));
        }
    }
}