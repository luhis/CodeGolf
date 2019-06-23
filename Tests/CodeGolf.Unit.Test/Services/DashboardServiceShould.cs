namespace CodeGolf.Unit.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Persistence.Static;
    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;

    using FluentAssertions;

    using Moq;

    using Optional;
    using Optional.Unsafe;

    using Xunit;

    public class DashboardServiceShould
    {
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);

        private readonly IDashboardService dashboardService;

        private readonly IGameRepository gameRepository;

        private readonly Mock<IAttemptRepository> attemptRepository;

        private readonly Mock<IHoleRepository> holeRepository;

        private readonly Mock<IUserRepository> userRepository;

        public DashboardServiceShould()
        {
            this.attemptRepository = this.mockRepository.Create<IAttemptRepository>();
            this.holeRepository = this.mockRepository.Create<IHoleRepository>();
            this.userRepository = this.mockRepository.Create<IUserRepository>();
            this.gameRepository = new GameRepository();
            this.dashboardService = new DashboardService(
                this.gameRepository,
                this.holeRepository.Object,
                null,
                this.attemptRepository.Object,
                this.userRepository.Object);
        }

        [Fact]
        public void GetFinalScoresNone()
        {
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(Task.FromResult<IReadOnlyList<Attempt>>(new Attempt[] { }));
            var scores = this.dashboardService.GetFinalScores(CancellationToken.None).Result;
            scores.Should().BeEquivalentTo();
        }

        [Fact]
        public void GetFinalScoresOneUser()
        {
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new Attempt[] { new Attempt(Guid.NewGuid(), "matt", Guid.NewGuid(), "", 11, DateTime.UtcNow), }));
            this.userRepository.Setup(a => a.GetByUserName("matt", CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));

            var scores = this.dashboardService.GetFinalScores(CancellationToken.None).Result;

            scores.Should().BeEquivalentTo(new ResultDto(1, "matt", "avatar.png", 6));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetAttemptsNone()
        {
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole()).Returns(
                Task.FromResult(Option.Some(new HoleInstance(hole.HoleId, DateTime.UtcNow, null))));
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(Task.FromResult<IReadOnlyList<Attempt>>(new Attempt[] { }));
            var scores = this.dashboardService.GetAttempts(CancellationToken.None).Result;
            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().HaveCount(0);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetAttemptsSome()
        {
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole()).Returns(
                Task.FromResult(Option.Some(new HoleInstance(hole.HoleId, DateTime.UtcNow, null))));
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new Attempt[] { new Attempt(Guid.NewGuid(), "matt", Guid.NewGuid(), "", 11, DateTime.UtcNow), }));
            this.userRepository.Setup(a => a.GetByUserName("matt", CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));
            var scores = this.dashboardService.GetAttempts(CancellationToken.None).Result;
            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().HaveCount(1);
            var first = scores.ValueOrFailure().First();
            first.LoginName.Should().Be("matt");
            first.Avatar.Should().Be("avatar.png");
            first.Score.Should().Be(11);
            this.mockRepository.VerifyAll();
        }
    }
}