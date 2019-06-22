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

    using Xunit;

    public class DashboardServiceShould
    {
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);
        private readonly IGameRepository gameRepository;
        private readonly IDashboardService dashboardService;

        private readonly Mock<IAttemptRepository> attemptRepository;

        public DashboardServiceShould()
        {
            this.gameRepository = new GameRepository();
            this.attemptRepository = this.mockRepository.Create<IAttemptRepository>();
            this.dashboardService = new DashboardService(this.gameRepository, null, null, this.attemptRepository.Object, null);
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
            this.attemptRepository.Setup(a => a.GetAttempts(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(Task.FromResult<IReadOnlyList<Attempt>>(new Attempt[] { new Attempt(Guid.NewGuid(), "matt", Guid.NewGuid(), "", 11, DateTime.UtcNow),  }));
            var scores = this.dashboardService.GetFinalScores(CancellationToken.None).Result;
            scores.Should().BeEquivalentTo( new ResultDto("matt", "", 6));
        }
    }
}