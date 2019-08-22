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

    public class ChallengeSetArrayShould
    {
        private readonly IReadOnlyList<ParamDescription> stringParam =
            new ParamDescription[] { new ParamDescription(typeof(string), "s"), };

        [Fact]
        public async Task ReturnFalseResultWhenIncorrectWithArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>(
                Guid.NewGuid(),
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string[]>(new object[] { "test" }, new string[] { "a" }) });
            var r = await a.GetResults(
                new CompileRunner(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>(new[] { "testXX" }) })));
            r.Single().Error.Should().NotBeNull();
        }

        [Fact]
        public async Task ReturnFalseResultWhenIncorrectWithEmptyArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>(
                Guid.NewGuid(),
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string[]>(new object[] { "test" }, new string[] { "a" }), });
            var r = await a.GetResults(
                new CompileRunner(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>(new string[] { }) })));
            r.Single().Error.Should().NotBeNull();
        }

        [Fact]
        public async Task ReturnFalseResultWhenIncorrectWithNullInArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>(
                Guid.NewGuid(),
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string[]>(new object[] { "test" }, new string[] { "a" }), });
            var r = await a.GetResults(
                new CompileRunner(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>(new string[] { null }) })));
            r.Single().Error.Should().NotBeNull();
        }

        [Fact]
        public async Task ReturnFalseResultWhenIncorrectWithNull()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>(
                Guid.NewGuid(),
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string[]>(new object[] { "test" }, new string[] { "a" }), });
            var r = await a.GetResults(
                new CompileRunner(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>(null) })));
            r.Single().Error.Should().NotBeNull();
        }
    }
}