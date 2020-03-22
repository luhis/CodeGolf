namespace CodeGolf.Unit.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Service;

    using FluentAssertions;
    using Moq;
    using Optional;
    using Xunit;

    public class GameServiceShould
    {
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);

        private readonly IGameService gameService;

        private readonly Mock<ICodeGolfService> codeGolfService;

        private readonly Mock<IAttemptRepository> attemptRepository;

        private readonly Mock<IGameRepository> gameRepository;

        private readonly Mock<IHoleRepository> holeRepository;

        private readonly Mock<IUserRepository> userRepository;

        private readonly Mock<IChallengeRepository> challengeRepository;

        private readonly Mock<ISignalRNotifier> signalRNotifier;

        public GameServiceShould()
        {
            this.codeGolfService = this.mockRepository.Create<ICodeGolfService>();
            this.attemptRepository = this.mockRepository.Create<IAttemptRepository>();
            this.gameRepository = this.mockRepository.Create<IGameRepository>();
            this.holeRepository = this.mockRepository.Create<IHoleRepository>();
            this.userRepository = this.mockRepository.Create<IUserRepository>();
            this.challengeRepository = this.mockRepository.Create<IChallengeRepository>();
            this.signalRNotifier = this.mockRepository.Create<ISignalRNotifier>();

            this.gameService = new GameService(
                this.codeGolfService.Object,
                this.attemptRepository.Object,
                this.gameRepository.Object,
                this.holeRepository.Object,
                this.signalRNotifier.Object,
                this.userRepository.Object,
                this.challengeRepository.Object);
        }

        [Fact]
        public async Task GetCurrentHoleNone()
        {
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None))
                .Returns(Task.FromResult(Option.None<HoleInstance>()));
            var r = await this.gameService.GetCurrentHole(CancellationToken.None);
            r.Should().NotBeNull();
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCurrentHoleSome()
        {
            var holeId = Guid.NewGuid();
            var challengeId = Guid.NewGuid();
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new HoleInstance(holeId, DateTime.UtcNow, null))));
            this.gameRepository.Setup(a => a.GetByHoleId(holeId, CancellationToken.None))
                .Returns(Option.Some(new Hole(holeId, challengeId, TimeSpan.FromMinutes(5), 1)));
            this.challengeRepository.Setup(a => a.GetById(challengeId, CancellationToken.None)).Returns(Option.Some<IChallengeSet>(new ChallengeSet<string>(challengeId, "title", "description", new List<ParamDescription>(), new List<Challenge<string>>())));
            this.gameRepository.Setup(a => a.GetAfter(holeId, CancellationToken.None))
                .Returns(Option.None<Hole>());

            var r = await this.gameService.GetCurrentHole(CancellationToken.None);
            r.Should().NotBeNull();
            this.mockRepository.VerifyAll();
        }
    }
}