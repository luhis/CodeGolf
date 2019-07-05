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
                this.userRepository.Object, 
                new BestAttemptsService(this.attemptRepository.Object));
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

            scores.ValueOrFailure().Should().BeEquivalentTo(new AttemptDto(1, id, "matt", "avatar.png", 11, "01/01/2010 00:00:00"));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetAttemptsOrderedByScore()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None)).Returns(
                Task.FromResult(Option.Some(new HoleInstance(hole.HoleId, DateTime.UtcNow, null))));
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(id1, 1, Guid.NewGuid(), string.Empty, 11, now),
                            new Attempt(id2, 2, Guid.NewGuid(), string.Empty, 12, now),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "avatar2.png"))));

            var scores = this.dashboardService.GetAttempts(CancellationToken.None).Result;

            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().HaveCount(2);
            scores.ValueOrFailure().Should().BeEquivalentTo(
                new AttemptDto(1, id1, "matt", "avatar.png", 11, now.ToLocalTime().ToString()),
                new AttemptDto(2, id2, "matt2", "avatar2.png", 12, now.ToLocalTime().ToString()));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetAttemptsOrderedByTime()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var date1 = new DateTime(2000, 1, 1, 2, 0, 0);
            var date2 = new DateTime(2000, 1, 1, 1, 0, 0);
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None)).Returns(
                Task.FromResult(Option.Some(new HoleInstance(hole.HoleId, DateTime.UtcNow, null))));
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(id1, 1, Guid.NewGuid(), string.Empty, 11, date1),
                            new Attempt(id2, 2, Guid.NewGuid(), string.Empty, 11, date2),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "avatar2.png"))));

            var scores = this.dashboardService.GetAttempts(CancellationToken.None).Result;

            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().BeEquivalentTo(
                new AttemptDto(1, id2, "matt2", "avatar2.png", 11, date2.ToLocalTime().ToString()),
                new AttemptDto(2, id1, "matt", "avatar.png", 11, date1.ToLocalTime().ToString()));

            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void ReturnEldestWhenUserHasIdenticalScores()
        {
            var id1 = Guid.NewGuid();
            var date1 = new DateTime(2000, 1, 1, 2, 0, 0);
            var date2 = new DateTime(2000, 1, 1, 1, 0, 0);
            var hole = this.gameRepository.GetGame().Holes.First();
            this.holeRepository.Setup(a => a.GetCurrentHole(CancellationToken.None)).Returns(
                Task.FromResult(Option.Some(new HoleInstance(hole.HoleId, DateTime.UtcNow, null))));
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(id1, 1, Guid.NewGuid(), string.Empty, 11, date1),
                            new Attempt(id1, 1, Guid.NewGuid(), string.Empty, 11, date2),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "avatar.png"))));

            var scores = this.dashboardService.GetAttempts(CancellationToken.None).Result;

            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().BeEquivalentTo(
                new AttemptDto(1, id1, "matt", "avatar.png", 11, date2.ToLocalTime().ToString()));

            this.mockRepository.VerifyAll();
        }
    }
}