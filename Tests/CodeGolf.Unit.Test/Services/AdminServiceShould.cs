namespace CodeGolf.Unit.Test.Services
{
    using System.Threading.Tasks;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Service;
    using Moq;
    using Xunit;

    public class AdminServiceShould
    {
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);

        private readonly IAdminService adminService;

        private readonly Mock<IChallengeRepository> challengeRepository;

        private readonly Mock<IHoleRepository> holeRepository;

        private readonly Mock<ISignalRNotifier> signalRNotifier;

        private readonly Mock<IAttemptRepository> attemptRepository;

        private readonly Mock<IUserRepository> userRepository;

        private readonly Mock<IGameRepository> gameRepository;

        public AdminServiceShould()
        {
            this.challengeRepository = this.mockRepository.Create<IChallengeRepository>();
            this.holeRepository = this.mockRepository.Create<IHoleRepository>();
            this.signalRNotifier = this.mockRepository.Create<ISignalRNotifier>();
            this.attemptRepository = this.mockRepository.Create<IAttemptRepository>();
            this.userRepository = this.mockRepository.Create<IUserRepository>();
            this.gameRepository = this.mockRepository.Create<IGameRepository>();

            this.adminService = new AdminService(
                this.holeRepository.Object,
                this.signalRNotifier.Object,
                this.attemptRepository.Object,
                this.userRepository.Object,
                this.challengeRepository.Object,
                this.gameRepository.Object);
        }

        [Fact]
        public async Task ResetGame()
        {
            this.attemptRepository.Setup(a => a.ClearAll()).Returns(Task.CompletedTask);
            this.holeRepository.Setup(a => a.ClearAll()).Returns(Task.CompletedTask);
            this.userRepository.Setup(a => a.ClearAll()).Returns(Task.CompletedTask);
            this.signalRNotifier.Setup(a => a.NewRound()).Returns(Task.CompletedTask);

            await this.adminService.ResetGame();
            this.mockRepository.VerifyAll();
        }
    }
}