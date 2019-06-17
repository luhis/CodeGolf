using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;
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
        private readonly IUserRepository userRepository;
        private readonly ISignalRNotifier signalRNotifier;

        public GameService(ICodeGolfService codeGolfService, IAttemptRepository attemptRepository,
            IGameRepository gameRepository, IHoleRepository holeRepository, ISignalRNotifier signalRNotifier, IUserRepository userRepository)
        {
            this.codeGolfService = codeGolfService;
            this.attemptRepository = attemptRepository;
            this.gameRepository = gameRepository;
            this.holeRepository = holeRepository;
            this.signalRNotifier = signalRNotifier;
            this.userRepository = userRepository;
        }

        private static T GetAfter<T>(IReadOnlyList<T> list, Func<T, bool> equals) =>
            list.SkipWhile(b => !equals(b)).SkipWhile(equals).ElementAt(0);

        private async Task<IReadOnlyList<AttemptDto>> GetBestAttempts(Guid holeId, CancellationToken cancellationToken)
        {
            var attempts = await this.attemptRepository.GetAttempts(holeId, cancellationToken);
            var rows = attempts.OrderBy(a => a.Score).GroupBy(a => a.UserId).Select(a => a.First())
                .OrderByDescending(a => a.Score);

            return await Task.WhenAll(rows.Select(
                async r =>
                    {
                        var avatar = (await this.userRepository.GetByUserName(r.UserId, cancellationToken)).Map(a => a.AvatarUri).ValueOr(string.Empty);
                        return new AttemptDto(r.Id, r.UserId, avatar, r.Score, r.TimeStamp);
                    }));
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
            return Task.FromResult(this.gameRepository.GetGame().Holes);
        }

        async Task<Option<Option<int, IReadOnlyList<ChallengeResult>>, ErrorSet>> IGameService.Attempt(User user,
            Guid holeId, string code,
            IChallengeSet challengeSet, CancellationToken cancellationToken)
        {
            var res = await this.codeGolfService.Score(code, challengeSet, cancellationToken);
            await res.Match(
                success => success.Match(
                    async score =>
                        {
                            //todo this order is a bit suspect
                            await this.userRepository.AddOrUpdate(user, cancellationToken);
                            if (score < await this.attemptRepository.GetBestScore(holeId, cancellationToken))
                            {
                                await this.signalRNotifier.NewTopScore(user.LoginName, score, user.AvatarUri);
                            }

                            await this.attemptRepository.AddAttempt(new Attempt(Guid.NewGuid(), user.LoginName, holeId, code, score,
                    DateTime.UtcNow));
                            await this.signalRNotifier.NewAnswer();
                        }, _ => Task.CompletedTask), _ => Task.CompletedTask);
            return res;
        }

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
            await this.userRepository.ClearAll();
            await this.signalRNotifier.NewRound();
        }

        async Task<Option<IReadOnlyList<AttemptDto>>> IGameService.GetAttempts(CancellationToken cancellationToken)
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
