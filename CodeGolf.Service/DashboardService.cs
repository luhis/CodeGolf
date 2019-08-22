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

        private readonly IChallengeRepository challengeRepository;

        private readonly IBestAttemptsService bestAttemptsService;

        public DashboardService(
            IGameRepository gameRepository,
            IHoleRepository holeRepository,
            ISignalRNotifier signalRNotifier,
            IAttemptRepository attemptRepository,
            IUserRepository userRepository,
            IBestAttemptsService bestAttemptsService,
            IChallengeRepository challengeRepository)
        {
            this.gameRepository = gameRepository;
            this.holeRepository = holeRepository;
            this.signalRNotifier = signalRNotifier;
            this.attemptRepository = attemptRepository;
            this.userRepository = userRepository;
            this.bestAttemptsService = bestAttemptsService;
            this.challengeRepository = challengeRepository;
        }

        async Task<Option<Guid>> IDashboardService.NextHole(Guid gameId, CancellationToken cancellationToken)
        {
            var next = await this.GetNextHole(gameId, cancellationToken);
            var id = await next.MapAsync(
                         async some =>
                             {
                                 some.SetStart(DateTime.UtcNow);
                                 await this.holeRepository.Update(some, cancellationToken);
                                 return some.HoleId;
                             });
            await this.signalRNotifier.NewRound();
            return id;
        }

        async Task IDashboardService.EndHole(Guid holeId, CancellationToken cancellationToken)
        {
            await this.holeRepository.EndHole(holeId, DateTime.UtcNow, cancellationToken);
            await this.signalRNotifier.NewRound();
        }

        async Task<Option<HoleDto>> IDashboardService.GetCurrentHole(CancellationToken cancellationToken)
        {
            var hole = await this.holeRepository.GetCurrentHole(cancellationToken);
            return hole.Map(
                a =>
                    {
                        var curr = this.gameRepository.GetByHoleId(a.HoleId, cancellationToken).ValueOrFailure();
                        var next = this.gameRepository.GetAfter(curr.HoleId, cancellationToken);
                        var chal = this.challengeRepository.GetById(curr.ChallengeId).ValueOrFailure();
                        return new HoleDto(curr, a.Start.Value, a.Start.Value.Add(curr.Duration), a.End, next.HasValue, chal);
                    });
        }

        async Task<Option<AttemptCodeDto>> IDashboardService.GetAttemptById(Guid attemptId, CancellationToken cancellationToken)
        {
            var attempt = await this.attemptRepository.GetAttempt(attemptId, cancellationToken);
            return await attempt.MapAsync(
                async a =>
                    {
                        var user = await this.userRepository.GetByUserId(a.UserId, cancellationToken);
                        return new AttemptCodeDto(a.Id, user.Map(x => x.LoginName).ValueOr(string.Empty), user.Map(x => x.AvatarUri.ToString()).ValueOr(string.Empty), a.Score, a.TimeStamp.ToLocalTime().ToString(), a.Code);
                    });
        }

        async Task<Option<IReadOnlyList<AttemptDto>>> IDashboardService.GetAttempts(
            Guid holeId,
            CancellationToken cancellationToken)
        {
            return Option.Some(await this.GetBestAttemptDtos(holeId, cancellationToken));
        }

        private async Task<Option<Hole>> GetNextHole(Guid gameId, CancellationToken cancellationToken)
        {
            var currentHole = await this.holeRepository.GetCurrentHole(cancellationToken);
            return await currentHole.Match(
                some => Task.FromResult(this.gameRepository.GetAfter(some.HoleId, cancellationToken)),
                async () => (await this.holeRepository.GetGameHoles(gameId, cancellationToken)).FirstOrNone());
        }

        private async Task<IReadOnlyList<AttemptDto>> GetBestAttemptDtos(
            Guid holeId,
            CancellationToken cancellationToken)
        {
            var rows = await this.bestAttemptsService.GetBestAttempts(holeId, cancellationToken);

            return await Task.WhenAll(
                       rows.Select(
                           async (r, i) =>
                           {
                               var user = await this.userRepository.GetByUserId(r.UserId, cancellationToken);
                               return new AttemptDto(i + 1, r.Id, user.Map(a => a.LoginName).ValueOr(string.Empty), user.Map(a => a.AvatarUri.ToString()).ValueOrDefault(), r.Score, r.TimeStamp);
                           }));
        }
    }
}
