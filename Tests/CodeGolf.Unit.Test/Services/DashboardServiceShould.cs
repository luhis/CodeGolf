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
                    new[] { new Attempt(Guid.NewGuid(), 1, Guid.NewGuid(), string.Empty, 11, DateTime.UtcNow), }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));

            var scores = this.dashboardService.GetFinalScores(CancellationToken.None).Result;

            scores.Should().BeEquivalentTo(new ResultDto(1, "matt", "avatar.png", 6));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetFinalScoresTwoUsers()
        {
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(Guid.NewGuid(), 1, Guid.NewGuid(), string.Empty, 11, DateTime.UtcNow),
                            new Attempt(Guid.NewGuid(), 2, Guid.NewGuid(), string.Empty, 12, DateTime.UtcNow),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "avatar2.png"))));

            var scores = this.dashboardService.GetFinalScores(CancellationToken.None).Result;

            scores.Should().BeEquivalentTo(new ResultDto(1, "matt", "avatar.png", 6), new ResultDto(2, "matt2", "avatar2.png", 4));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetAttemptsNone()
        {
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None)).Returns(
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
            var id = Guid.NewGuid();
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None)).Returns(
                Task.FromResult(Option.Some(new HoleInstance(hole.HoleId, DateTime.UtcNow, null))));
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new Attempt[] { new Attempt(id, 1, Guid.NewGuid(), string.Empty, 11, new DateTime(2010, 1, 1)), }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));

            var scores = this.dashboardService.GetAttempts(CancellationToken.None).Result;

            scores.ValueOrFailure().Should().BeEquivalentTo(new AttemptDto(1, id, 1, "avatar.png", 11, "01/01/2010 00:00:00"));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetAttemptsOrderedByScore()
        {
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None)).Returns(
                Task.FromResult(Option.Some(new HoleInstance(hole.HoleId, DateTime.UtcNow, null))));
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(Guid.NewGuid(), 1, Guid.NewGuid(), string.Empty, 11, DateTime.UtcNow),
                            new Attempt(Guid.NewGuid(), 2, Guid.NewGuid(), string.Empty, 12, DateTime.UtcNow),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "avatar2.png"))));
            var scores = this.dashboardService.GetAttempts(CancellationToken.None).Result;
            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().HaveCount(2);
            var first = scores.ValueOrFailure().First();
            first.UserId.Should().Be(1);
            first.Avatar.Should().Be("avatar.png");
            first.Score.Should().Be(11);
            var second = scores.ValueOrFailure()[1];
            second.UserId.Should().Be(2);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetAttemptsOrderedByTime()
        {
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None)).Returns(
                Task.FromResult(Option.Some(new HoleInstance(hole.HoleId, DateTime.UtcNow, null))));
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(Guid.NewGuid(), 1, Guid.NewGuid(), string.Empty, 11, new DateTime(2000, 1, 1, 2, 0, 0)),
                            new Attempt(Guid.NewGuid(), 2, Guid.NewGuid(), string.Empty, 11, new DateTime(2000, 1, 1, 1, 0, 0)),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "avatar2.png"))));

            var scores = this.dashboardService.GetAttempts(CancellationToken.None).Result;

            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().HaveCount(2);
            var first = scores.ValueOrFailure().First();
            first.UserId.Should().Be(2);
            first.Avatar.Should().Be("avatar2.png");
            first.Score.Should().Be(11);
            var second = scores.ValueOrFailure()[1];
            second.UserId.Should().Be(1);
            this.mockRepository.VerifyAll();
        }
    }
}