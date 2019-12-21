namespace CodeGolf.Unit.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Persistence.Repositories;
    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Service.Handlers;
    using FluentAssertions;
    using MediatR;
    using Moq;

    using Optional;

    using Xunit;

    public class ResultsServiceShould
    {
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);

        private readonly IRequestHandler<FinalScores, IReadOnlyList<ResultDto>> dashboardService;

        private readonly IGameRepository gameRepository;

        private readonly Mock<IAttemptRepository> attemptRepository;

        private readonly Mock<IUserRepository> userRepository;

        public ResultsServiceShould()
        {
            this.attemptRepository = this.mockRepository.Create<IAttemptRepository>();
            this.userRepository = this.mockRepository.Create<IUserRepository>();
            this.gameRepository = new GameRepository();
            this.dashboardService = new FinalScoresHandler(
                this.gameRepository,
                this.userRepository.Object,
                new BestAttemptsService(this.attemptRepository.Object));
        }

        [Fact]
        public async Task GetFinalScoresNone()
        {
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(Task.FromResult<IReadOnlyList<Attempt>>(new Attempt[] { }));
            var scores = await this.dashboardService.Handle(new FinalScores(), CancellationToken.None);
            scores.Should().BeEquivalentTo();
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetFinalScoresOneUser()
        {
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[] { new Attempt(Guid.NewGuid(), 1, Guid.NewGuid(), string.Empty, 11, DateTime.UtcNow), }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "matt mccorry", new Uri("http://a.com/avatar.png")))));

            var scores = await this.dashboardService.Handle(new FinalScores(), CancellationToken.None);

            scores.Should().BeEquivalentTo(new ResultDto(1, "matt", "http://a.com/avatar.png", 6));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetFinalScoresTwoUsers()
        {
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None)).Returns(
                Task.FromResult<IReadOnlyList<Attempt>>(
                    new[]
                        {
                            new Attempt(Guid.NewGuid(), 1, Guid.NewGuid(), string.Empty, 11, DateTime.UtcNow),
                            new Attempt(Guid.NewGuid(), 2, Guid.NewGuid(), string.Empty, 12, DateTime.UtcNow),
                        }));
            this.userRepository.Setup(a => a.GetByUserId(1, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(1, "matt", "matt mccorry", new Uri("http://a.com/avatar.png")))));
            this.userRepository.Setup(a => a.GetByUserId(2, CancellationToken.None))
                .Returns(Task.FromResult(Option.Some(new User(2, "matt2", "matt2 mccorry", new Uri("http://a.com/avatar2.png")))));

            var scores = await this.dashboardService.Handle(new FinalScores(), CancellationToken.None);

            scores.Should().BeEquivalentTo(new ResultDto(1, "matt", "http://a.com/avatar.png", 6), new ResultDto(2, "matt2", "http://a.com/avatar2.png", 4));
            this.mockRepository.VerifyAll();
        }
    }
}