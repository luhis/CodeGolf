namespace CodeGolf.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Domain.Repositories;

    public class AdminService : IAdminService
    {
        private readonly IChallengeRepository challengeRepository;

        private readonly IHoleRepository holeRepository;

        private readonly ISignalRNotifier signalRNotifier;

        private readonly IAttemptRepository attemptRepository;

        private readonly IUserRepository userRepository;

        public AdminService(
            IHoleRepository holeRepository,
            ISignalRNotifier signalRNotifier,
            IAttemptRepository attemptRepository,
            IUserRepository userRepository,
            IChallengeRepository challengeRepository)
        {
            this.holeRepository = holeRepository;
            this.signalRNotifier = signalRNotifier;
            this.attemptRepository = attemptRepository;
            this.userRepository = userRepository;
            this.challengeRepository = challengeRepository;
        }

        async Task IAdminService.ResetGame()
        {
            await this.attemptRepository.ClearAll();
            await this.holeRepository.ClearAll();
            await this.userRepository.ClearAll();
            await this.signalRNotifier.NewRound();
        }

        Task<IReadOnlyList<IChallengeSet>> IAdminService.GetAllHoles(CancellationToken cancellationToken)
        {
            var holes = this.challengeRepository.GetAll();
            return Task.FromResult(holes);
        }

        Task<IReadOnlyList<IChallengeSet>> IAdminService.GetAllChallenges(in CancellationToken cancellationToken)
        {
            return Task.FromResult(this.challengeRepository.GetAll());
        }
    }
}
