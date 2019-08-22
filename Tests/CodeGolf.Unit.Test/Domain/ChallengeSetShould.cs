namespace CodeGolf.Unit.Test.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using FluentAssertions;
    using Optional;
    using Xunit;

    public class ChallengeSetShould
    {
        private readonly IReadOnlyList<ParamDescription> stringParam =
            new ParamDescription[] { new ParamDescription(typeof(string), "s"), };

        [Fact]
        public void ThrowWhenTheChallengeParamsAreMismatched()
        {
            Action a = () => new ChallengeSet<string>(
                Guid.NewGuid(),
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string>(new object[] { 42 }, "test") });
            a.Should().Throw<Exception>().WithMessage("Mismatched parameters");
        }

        [Fact]
        public void NotThrowWhenTheChallengeParamsAreNotMismatched()
        {
            var a = new ChallengeSet<string>(
                Guid.NewGuid(),
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string>(new object[] { "test" }, "test") });
            a.Should().NotBeNull();
        }

        [Fact]
        public async Task ReturnTrueResultWhenCorrect()
        {
            var a = (IChallengeSet)new ChallengeSet<string>(
                Guid.NewGuid(),
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string>(new object[] { "test" }, "test") });
            var r = await a.GetResults(
                new CompileRunner(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>("test") })));
            r.Single().Error.Should().BeNull();
        }

        [Fact]
        public async Task ReturnFalseResultWhenIncorrect()
        {
            var a = (IChallengeSet)new ChallengeSet<string>(
                Guid.NewGuid(),
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string>(new object[] { "test" }, "test") });
            var r = await a.GetResults(
                new CompileRunner(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>("testXX") })));
            r.Single().Error.Should().NotBeNull();
        }
    }
}