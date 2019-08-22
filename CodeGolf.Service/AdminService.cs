namespace CodeGolf.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Service.Dtos;

    public class AdminService : IAdminService
    {
        private readonly IChallengeRepository challengeRepository;

        private readonly IHoleRepository holeRepository;

        private readonly ISignalRNotifier signalRNotifier;

        private readonly IAttemptRepository attemptRepository;

        private readonly IUserRepository userRepository;

        private readonly IGameRepository gameRepository;

        public AdminService(
            IHoleRepository holeRepository,
            ISignalRNotifier signalRNotifier,
            IAttemptRepository attemptRepository,
            IUserRepository userRepository,
            IChallengeRepository challengeRepository,
            IGameRepository gameRepository)
        {
            this.holeRepository = holeRepository;
            this.signalRNotifier = signalRNotifier;
            this.attemptRepository = attemptRepository;
            this.userRepository = userRepository;
            this.challengeRepository = challengeRepository;
            this.gameRepository = gameRepository;
        }

        async Task IAdminService.ResetGame(CancellationToken cancellationToken)
        {
            await this.attemptRepository.ClearAll(cancellationToken);
            await this.holeRepository.ClearAll(cancellationToken);
            await this.userRepository.ClearAll(cancellationToken);
            await this.signalRNotifier.NewRound();
        }

        Task<IReadOnlyList<IChallengeSet>> IAdminService.GetAllChallenges(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.challengeRepository.GetAll());
        }

        Task IAdminService.CreateGame(GameDto challenges, string accessKey, User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        async Task<IReadOnlyList<GameDto>> IAdminService.GetAllGames(User user, CancellationToken cancellationToken)
        {
            return await Task.WhenAll(this.gameRepository.GetMyGames(user.UserId, cancellationToken).Select(async a =>
            {
                var holes = await this.holeRepository.GetGameHoles(a.Id, cancellationToken);
                return new GameDto(a.Id, a.AccessKey, holes.Select(b => new RoundDto(b.ChallengeId, "a")).ToList());
            }));
        }
    }
}
