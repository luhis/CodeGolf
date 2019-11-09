namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Service.Dtos;
    using OneOf;
    using Optional;
    using Optional.Unsafe;

    public class GameService : IGameService
    {
        private readonly ICodeGolfService codeGolfService;

        private readonly IAttemptRepository attemptRepository;

        private readonly IGameRepository gameRepository;

        private readonly IHoleRepository holeRepository;

        private readonly IUserRepository userRepository;

        private readonly IChallengeRepository challengeRepository;

        private readonly ISignalRNotifier signalRNotifier;

        public GameService(
            ICodeGolfService codeGolfService,
            IAttemptRepository attemptRepository,
            IGameRepository gameRepository,
            IHoleRepository holeRepository,
            ISignalRNotifier signalRNotifier,
            IUserRepository userRepository,
            IChallengeRepository challengeRepository)
        {
            this.codeGolfService = codeGolfService;
            this.attemptRepository = attemptRepository;
            this.gameRepository = gameRepository;
            this.holeRepository = holeRepository;
            this.signalRNotifier = signalRNotifier;
            this.userRepository = userRepository;
            this.challengeRepository = challengeRepository;
        }

        async Task<Option<HoleDto>> IGameService.GetCurrentHole(CancellationToken cancellationToken)
        {
            var hole = await this.holeRepository.GetCurrentHole(cancellationToken);
            return hole.Match(
                a =>
                    {
                        if (!a.End.HasValue)
                        {
                            var curr = this.gameRepository.GetByHoleId(a.HoleId);

                            return curr.Map(
                                x =>
                                {
                                    var chal = this.challengeRepository.GetById(x.ChallengeId).ValueOrFailure();
                                    var next = this.gameRepository.GetAfter(x.HoleId);
                                    return new HoleDto(x, a.Start, a.Start.Add(x.Duration), a.End, next.HasValue, chal);
                                    });
                        }
                        else
                        {
                            return Option.None<HoleDto>();
                        }
                    },
                Option.None<HoleDto>);
        }

        async Task<OneOf<int, IReadOnlyList<ChallengeResult>, IReadOnlyList<CompileErrorMessage>>> IGameService.Attempt(
            User user,
            Guid holeId,
            string code,
            IChallengeSet challengeSet,
            CancellationToken cancellationToken)
        {
            var res = await this.codeGolfService.Score(code, challengeSet, cancellationToken);
            await res.Match(
                async score =>
                    {
                        await this.userRepository.AddOrUpdate(user, cancellationToken);

                        var newId = Guid.NewGuid();

                        await this.attemptRepository.AddAttempt(
                            new Attempt(newId, user.UserId, holeId, code, score, DateTime.UtcNow));
                        await this.signalRNotifier.NewAnswer();
                        if (await this.IsBestScore(holeId, newId, cancellationToken))
                        {
                            await this.signalRNotifier.NewTopScore(user.LoginName, score, user.AvatarUri.ToString());
                        }
                    },
                _ => Task.CompletedTask,
                _ => Task.CompletedTask);
            return res;
        }

        private async Task<IOrderedEnumerable<Attempt>> GetBestAttempts(
            Guid holeId,
            CancellationToken cancellationToken)
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
    }
}
