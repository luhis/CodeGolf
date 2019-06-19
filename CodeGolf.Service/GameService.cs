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

        private async Task<IOrderedEnumerable<Attempt>> GetBestAttempts(Guid holeId, CancellationToken cancellationToken)
        {
            var attempts = await this.attemptRepository.GetAttempts(holeId, cancellationToken);
            return attempts.OrderBy(a => a.Score).GroupBy(a => a.UserId).Select(a => a.First())
                .OrderByDescending(a => a.Score);
        }

        private async Task<bool> IsBestScore(Guid holeId, Guid attemptId, CancellationToken cancellationToken)
        {
            var bests = await this.GetBestAttempts(holeId, cancellationToken);
            if (bests.Any())
            {
                return bests.First().Id == attemptId;
            }
            else
            {
                return true;
            }
        }

        private async Task<IReadOnlyList<AttemptDto>> GetBestAttemptDtos(Guid holeId, CancellationToken cancellationToken)
        {
            var rows = await this.GetBestAttempts(holeId, cancellationToken);

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

        private static int PosToPoints(int i)
        {
            switch (i)
            {
                case 1:
                    return 3;
                case 2:
                    return 2;
                case 3:
                    return 1;
                default:
                    return 0;
            }
        }

        async Task<IReadOnlyList<ResultDto>> IGameService.GetFinalScores(CancellationToken cancellationToken)
        {
            var holes = await Task.WhenAll(this.gameRepository.GetGame().Holes.Select(async h => (await this.GetBestAttempts(h.HoleId, cancellationToken)).Select((a, b) => Tuple.Create(b, a))));
            var ranks = holes.SelectMany(a => a);
            return ranks.GroupBy(a => a.Item2.UserId).Select(r => new ResultDto(r.Key, r.Sum(a => PosToPoints(a.Item1)))).ToList();
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
                            await this.userRepository.AddOrUpdate(user, cancellationToken);

                            var newId = Guid.NewGuid();

                            await this.attemptRepository.AddAttempt(new Attempt(newId, user.LoginName, holeId, code, score, DateTime.UtcNow));
                            await this.signalRNotifier.NewAnswer();
                            if (await this.IsBestScore(holeId, newId, cancellationToken))
                            {
                                await this.signalRNotifier.NewTopScore(user.LoginName, score, user.AvatarUri);
                            }
                        },
                    _ => Task.CompletedTask),
                _ => Task.CompletedTask);
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
            return await hole.MapAsync(a => this.GetBestAttemptDtos(a.HoleId, cancellationToken));
        }

        Task<Attempt> IGameService.GetAttemptById(Guid attemptId, CancellationToken cancellationToken)
        {
            return this.attemptRepository.GetAttempt(attemptId, cancellationToken);
        }
    }
}
