﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly IHoleRepository holeRepository;
        private readonly ISignalRNotifier signalRNotifier;

        public GameService(ICodeGolfService codeGolfService, IAttemptRepository attemptRepository,
            IGameRepository gameRepository, IHoleRepository holeRepository, ISignalRNotifier signalRNotifier)
        {
            this.codeGolfService = codeGolfService;
            this.attemptRepository = attemptRepository;
            this.gameRepository = gameRepository;
            this.holeRepository = holeRepository;
            this.signalRNotifier = signalRNotifier;
        }

        private async Task<IReadOnlyList<Attempt>> GetBestAttempts(Guid holeId, CancellationToken cancellationToken)
        {
            var attempts = await this.attemptRepository.GetAttempts(holeId, cancellationToken);
            return attempts.OrderBy(a => a.Score).GroupBy(a => a.UserId).Select(a => a.First())
                .OrderByDescending(a => a.Score).ToList();
        }

        async Task<Option<HoleDto>> IGameService.GetCurrentHole(CancellationToken cancellationToken)
        {
            var hole = await this.holeRepository.GetCurrentHole();
            return hole.Map(a =>
            {
                var curr = this.gameRepository.GetGame().Holes.First(b => b.HoleId.Equals(a.HoleId));
                return new HoleDto(curr, a.Start, a.End);
            });
        }

        Task<IReadOnlyList<Hole>> IGameService.GetAllHoles(CancellationToken cancellationToken)
        {
                return Task.FromResult<IReadOnlyList<Hole>>(this.gameRepository.GetGame().Holes);
        }

        async Task<Option<Option<int, IReadOnlyList<ChallengeResult>>, ErrorSet>> IGameService.Attempt(string userId,
            Guid holeId, string code,
            IChallengeSet challengeSet, CancellationToken cancellationToken)
        {
            var res = await this.codeGolfService.Score(code, challengeSet, cancellationToken);
            await res.Match(success => success.Match(async score =>
            {
                await this.signalRNotifier.NewAnswer();
                if (score < await this.attemptRepository.GetBestScore(holeId, cancellationToken))
                {
                    await this.signalRNotifier.NewTopScore(userId, score);
                }
                await this.attemptRepository.AddAttempt(new Attempt(Guid.NewGuid(), userId, holeId, code, score,
                    DateTime.UtcNow));
            }, _ => Task.CompletedTask), _ => Task.CompletedTask);
            return res;
        }

        private static T GetAfter<T>(IReadOnlyList<T> list, Func<T, bool> equals) =>
            list.SkipWhile(b => !equals(b)).SkipWhile(equals).ElementAt(0);

        async Task IGameService.NextRound()
        {
            var round = await this.holeRepository.GetCurrentHole();
            var next = round.Match(
                some => GetAfter(this.gameRepository.GetGame().Holes, item => item.HoleId.Equals(some.HoleId)),
                () => this.gameRepository.GetGame().Holes.First());
            await this.holeRepository.AddHole(new HoleInstance(next.HoleId, DateTime.UtcNow,
                DateTime.UtcNow.Add(next.Duration)));
            await this.signalRNotifier.NewRound();
        }

        async Task IGameService.ResetGame()
        {
            await this.attemptRepository.ClearAll();
            await this.holeRepository.ClearAll();
            await this.signalRNotifier.NewRound();
        }

        async Task<Option<IReadOnlyList<Attempt>>> IGameService.GetAttempts(CancellationToken cancellationToken)
        {
            var hole = await this.holeRepository.GetCurrentHole();
            return await hole.MapAsync(a => this.GetBestAttempts(a.HoleId, cancellationToken));
        }

        Task<Attempt> IGameService.GetAttemptById(Guid attemptId, CancellationToken cancellationToken)
        {
            return this.attemptRepository.GetAttempt(attemptId, cancellationToken);
        }
    }
}
