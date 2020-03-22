namespace CodeGolf.Unit.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Persistence.Repositories;
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
                new BestAttemptsService(this.attemptRepository.Object),
                new ChallengeRepository());
        }

        [Fact]
        public async Task GetAttemptsNone()
        {
            var hole = this.gameRepository.GetGame(CancellationToken.None).Holes.First();
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(Task.FromResult<IReadOnlyList<Attempt>>(new Attempt[] { }));
            var scores = await this.dashboardService.GetAttempts(hole.HoleId, CancellationToken.None);
            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().HaveCount(0);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAttemptsSome()
        {
            var id = Guid.NewGuid();
            var hole = this.gameRepository.GetGame(CancellationToken.None).Holes.First();
            var date = new DateTime(2010, 1, 1);
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new Attempt[] { new Attempt(id, 1, Guid.NewGuid(), string.Empty, 11, date), }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "matt mccorry", new Uri("http://a.com/avatar.png")))));

            var scores = await this.dashboardService.GetAttempts(hole.HoleId, CancellationToken.None);

            scores.ValueOrFailure().Should().BeEquivalentTo(new AttemptDto(1, id, "matt", "http://a.com/avatar.png", 11, date));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAttemptsOrderedByScore()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var hole = this.gameRepository.GetGame(CancellationToken.None).Holes.First();
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(id1, 1, Guid.NewGuid(), string.Empty, 11, now),
                            new Attempt(id2, 2, Guid.NewGuid(), string.Empty, 12, now),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "matt mccorry", new Uri("http://a.com/avatar.png")))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "matt2 mccorry", new Uri("http://a.com/avatar2.png")))));

            var scores = await this.dashboardService.GetAttempts(hole.HoleId, CancellationToken.None);

            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().HaveCount(2);
            scores.ValueOrFailure().Should().BeEquivalentTo(
                new AttemptDto(1, id1, "matt", "http://a.com/avatar.png", 11, now),
                new AttemptDto(2, id2, "matt2", "http://a.com/avatar2.png", 12, now));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAttemptsOrderedByTime()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var date1 = new DateTime(2000, 1, 1, 2, 0, 0);
            var date2 = new DateTime(2000, 1, 1, 1, 0, 0);
            var hole = this.gameRepository.GetGame(CancellationToken.None).Holes.First();
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(id1, 1, Guid.NewGuid(), string.Empty, 11, date1),
                            new Attempt(id2, 2, Guid.NewGuid(), string.Empty, 11, date2),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "matt mccorry", new Uri("http://a.com/avatar.png")))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "matt2 mccorry", new Uri("http://a.com/avatar2.png")))));

            var scores = await this.dashboardService.GetAttempts(hole.HoleId, CancellationToken.None);

            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().BeEquivalentTo(
                new AttemptDto(1, id2, "matt2", "http://a.com/avatar2.png", 11, date2),
                new AttemptDto(2, id1, "matt", "http://a.com/avatar.png", 11, date1));

            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task ReturnEldestWhenUserHasIdenticalScores()
        {
            var id1 = Guid.NewGuid();
            var date1 = new DateTime(2000, 1, 1, 2, 0, 0);
            var date2 = new DateTime(2000, 1, 1, 1, 0, 0);
            var hole = this.gameRepository.GetGame(CancellationToken.None).Holes.First();
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(id1, 1, Guid.NewGuid(), string.Empty, 11, date1),
                            new Attempt(id1, 1, Guid.NewGuid(), string.Empty, 11, date2),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "matt mccorry", new Uri("http://a.com/avatar.png")))));

            var scores = await this.dashboardService.GetAttempts(hole.HoleId, CancellationToken.None);

            scores.HasValue.Should().BeTrue();
            scores.ValueOrFailure().Should().BeEquivalentTo(
                new AttemptDto(1, id1, "matt", "http://a.com/avatar.png", 11, date2));

            this.mockRepository.VerifyAll();
        }
    }
}