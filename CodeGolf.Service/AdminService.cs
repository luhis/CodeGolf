namespace CodeGolf.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;

    public class AdminService : IAdminService
    {
        private readonly IGameRepository gameRepository;

        private readonly IHoleRepository holeRepository;

        private readonly ISignalRNotifier signalRNotifier;

        private readonly IAttemptRepository attemptRepository;

        private readonly IUserRepository userRepository;

        public AdminService(IHoleRepository holeRepository, ISignalRNotifier signalRNotifier, IAttemptRepository attemptRepository, IUserRepository userRepository, IGameRepository gameRepository)
        {
            this.holeRepository = holeRepository;
            this.signalRNotifier = signalRNotifier;
            this.attemptRepository = attemptRepository;
            this.userRepository = userRepository;
            this.gameRepository = gameRepository;
        }

        async Task IAdminService.ResetGame()
        {
            await this.attemptRepository.ClearAll();
            await this.holeRepository.ClearAll();
            await this.userRepository.ClearAll();
            await this.signalRNotifier.NewRound();
        }

        Task<IReadOnlyList<Hole>> IAdminService.GetAllHoles(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.gameRepository.GetGame().Holes);
        }
    }
}