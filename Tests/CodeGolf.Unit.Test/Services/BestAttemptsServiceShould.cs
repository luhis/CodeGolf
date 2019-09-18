namespace CodeGolf.Unit.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Service;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class BestAttemptsServiceShould
    {
        private readonly IBestAttemptsService codeGolfService;

        private readonly Mock<IAttemptRepository> attemptRepository;
        private readonly MockRepository mr = new MockRepository(MockBehavior.Strict);

        public BestAttemptsServiceShould()
        {
            this.attemptRepository = this.mr.Create<IAttemptRepository>();
            this.codeGolfService = new BestAttemptsService(this.attemptRepository.Object);
        }

        [Fact]
        public async Task ReturnCorrectResultForHelloWorld()
        {
            var id = Guid.NewGuid();
            this.attemptRepository.Setup(a => a.GetAttempts(id, CancellationToken.None))
                .Returns(Task.FromResult<IReadOnlyList<Attempt>>(new[]
                {
                    new Attempt(Guid.NewGuid(), 1, id, "code", 11, new DateTime(2000, 1, 1))
                }));

            var r = await this.codeGolfService.GetBestAttempts(
                id,
                CancellationToken.None);
            r.Should().NotBeNull();
            this.mr.VerifyAll();
        }
    }
}
