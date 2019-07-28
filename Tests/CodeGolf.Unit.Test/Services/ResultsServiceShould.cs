namespace CodeGolf.Unit.Test.Services
{
    using System;
    using System.Collections.Generic;
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

    using Xunit;

    public class ResultsServiceShould
    {
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);

        private readonly IResultsService dashboardService;

        private readonly IGameRepository gameRepository;

        private readonly Mock<IAttemptRepository> attemptRepository;

        private readonly Mock<IUserRepository> userRepository;

        public ResultsServiceShould()
        {
            this.attemptRepository = this.mockRepository.Create<IAttemptRepository>();
            this.userRepository = this.mockRepository.Create<IUserRepository>();
            this.gameRepository = new GameRepository();
            this.dashboardService = new ResultsService(
                this.gameRepository,
                this.userRepository.Object,
                new BestAttemptsService(this.attemptRepository.Object));
        }

        [Fact]
        public void GetFinalScoresNone()
        {
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(Task.FromResult<IReadOnlyList<Attempt>>(new Attempt[] { }));
            var scores = this.dashboardService.GetFinalScores(CancellationToken.None).Result;
            scores.Should().BeEquivalentTo();
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetFinalScoresOneUser()
        {
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[] { new Attempt(Guid.NewGuid(), 1, Guid.NewGuid(), string.Empty, 11, DateTime.UtcNow), }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "matt mccorry", "avatar.png"))));

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
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "matt mccorry", "avatar.png"))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "matt2 mccorry", "avatar2.png"))));

            var scores = this.dashboardService.GetFinalScores(CancellationToken.None).Result;

            scores.Should().BeEquivalentTo(new ResultDto(1, "matt", "avatar.png", 6), new ResultDto(2, "matt2", "avatar2.png", 4));
            this.mockRepository.VerifyAll();
        }
    }
}