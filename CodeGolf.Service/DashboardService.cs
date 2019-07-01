namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Service.Dtos;

    using Optional;
    using Optional.Async;
    using Optional.Collections;
    using Optional.Unsafe;

    public class DashboardService : IDashboardService
    {
        private readonly IGameRepository gameRepository;

        private readonly IHoleRepository holeRepository;

        private readonly ISignalRNotifier signalRNotifier;

        private readonly IAttemptRepository attemptRepository;

        private readonly IUserRepository userRepository;

        public DashboardService(
            IGameRepository gameRepository,
            IHoleRepository holeRepository,
            ISignalRNotifier signalRNotifier,
            IAttemptRepository attemptRepository,
            IUserRepository userRepository)
        {
            this.gameRepository = gameRepository;
            this.holeRepository = holeRepository;
            this.signalRNotifier = signalRNotifier;
            this.attemptRepository = attemptRepository;
            this.userRepository = userRepository;
        }

        async Task<Option<Guid>> IDashboardService.NextHole(CancellationToken cancellationToken)
        {
            var next = await this.GetNextHole(cancellationToken);
            var id = await next.MapAsync(
                         async some =>
                             {
                                 await this.holeRepository.AddHole(
                                     new HoleInstance(some.HoleId, DateTime.UtcNow, null));
                                 return some.HoleId;
                             });
            await this.signalRNotifier.NewRound();
            return id;
        }

        async Task IDashboardService.EndHole(Guid holeId)
        {
            await this.holeRepository.EndHole(holeId, DateTime.UtcNow);
            await this.signalRNotifier.NewRound();
        }

        async Task<Option<HoleDto>> IDashboardService.GetCurrentHole(CancellationToken cancellationToken)
        {
            var hole = await this.holeRepository.GetCurrentHole(cancellationToken);
            return hole.Map(
                a =>
                    {
                        var curr = this.gameRepository.GetById(a.HoleId).ValueOrFailure();
                        var next = this.gameRepository.GetAfter(curr.HoleId);
                        return new HoleDto(curr, a.Start, a.Start.Add(curr.Duration), a.End, next.HasValue);
                    });
        }

        Task<IReadOnlyList<Hole>> IDashboardService.GetAllHoles(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.gameRepository.GetGame().Holes);
        }

        async Task IDashboardService.ResetGame()
        {
            await this.attemptRepository.ClearAll();
            await this.holeRepository.ClearAll();
            await this.userRepository.ClearAll();
            await this.signalRNotifier.NewRound();
        }

        private async Task<Option<Hole>> GetNextHole(CancellationToken cancellationToken)
        {
            var currentHole = await this.holeRepository.GetCurrentHole(cancellationToken);
            return currentHole.Match(
                some => this.gameRepository.GetAfter(some.HoleId),
                () => this.gameRepository.GetGame().Holes.FirstOrNone());
        }

        private static int PosToPoints(int i)
        {
            switch (i)
            {
                case 0:
                    return 3;
                case 1:
                    return 2;
                case 2:
                    return 1;
                default:
                    return 0;
            }
        }

        async Task<IReadOnlyList<ResultDto>> IDashboardService.GetFinalScores(CancellationToken cancellationToken)
        {
            var holes = await Task.WhenAll(
                            this.gameRepository.GetGame().Holes.Select(
                                async h => (await this.GetBestAttempts(h.HoleId, cancellationToken)).Select(
                                    (a, b) => ValueTuple.Create(b, a))));
            var ranks = holes.SelectMany(a => a);
            return (await Task.WhenAll(
                        ranks.GroupBy(a => a.Item2.UserId).Select(
                            async (r, i) =>
                                {
                                    var user = await this.userRepository.GetByUserId(r.Key, cancellationToken);
                                    return new ResultDto(
                                        i + 1,
                                        user.Map(a => a.LoginName).ValueOrDefault(),
                                        user.Match(a => a.AvatarUri, () => string.Empty),
                                        r.Sum(a => PosToPoints(a.Item1)));
                                }))).ToList();
        }

        private async Task<IOrderedEnumerable<Attempt>> GetBestAttempts(
            Guid holeId,
            CancellationToken cancellationToken)
        {
            var attempts = await this.attemptRepository.GetAttempts(holeId, cancellationToken);
            return attempts.OrderBy(a => a.Score).GroupBy(a => a.UserId).Select(a => a.First())
                .OrderBy(a => a.Score).ThenBy(a => a.TimeStamp);
        }

        private async Task<IReadOnlyList<AttemptDto>> GetBestAttemptDtos(
            Guid holeId,
            CancellationToken cancellationToken)
        {
            var rows = await this.GetBestAttempts(holeId, cancellationToken);

            return await Task.WhenAll(
                       rows.Select(
                           async (r, i) =>
                               {
                                   var avatar =
                                       (await this.userRepository.GetByUserId(r.UserId, cancellationToken))
                                       .Map(a => a.AvatarUri).ValueOr(string.Empty);
                                   return new AttemptDto(i + 1, r.Id, r.UserId, avatar, r.Score, r.TimeStamp.ToLocalTime().ToString());
                               }));
        }

        Task<Attempt> IDashboardService.GetAttemptById(Guid attemptId, CancellationToken cancellationToken)
        {
            return this.attemptRepository.GetAttempt(attemptId, cancellationToken);
        }

        async Task<Option<IReadOnlyList<AttemptDto>>> IDashboardService.GetAttempts(CancellationToken cancellationToken)
        {
            var hole = await this.holeRepository.GetCurrentHole(cancellationToken);
            return await hole.MapAsync(a => this.GetBestAttemptDtos(a.HoleId, cancellationToken));
        }
    }
}
