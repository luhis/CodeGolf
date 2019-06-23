using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;
using FluentAssertions;
using Optional;
using Xunit;

namespace CodeGolf.Unit.Test.Domain
{
    public class ChallengeSetArrayShould
    {
        private readonly IReadOnlyList<ParamDescription> stringParam =
            new ParamDescription[] { new ParamDescription(typeof(string), "s"), };

        [Fact]
        public void ReturnFalseResultWhenIncorrectWithArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>(
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string[]>(new object[] { "test" }, new string[] { "a" }) });
            var r = a.GetResults(
                new CompileResult(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>(new[] { "testXX" }) }))).Result;
            r.Single().Error.HasValue.Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseResultWhenIncorrectWithEmptyArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>(
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string[]>(new object[] { "test" }, new string[] { "a" }), });
            var r = a.GetResults(
                new CompileResult(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>(new string[] { }) }))).Result;
            r.Single().Error.HasValue.Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseResultWhenIncorrectWithNullInArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>(
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string[]>(new object[] { "test" }, new string[] { "a" }), });
            var r = a.GetResults(
                new CompileResult(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>(new string[] { null }) }))).Result;
            r.Single().Error.HasValue.Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseResultWhenIncorrectWithNull()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>(
                "a",
                "b",
                this.stringParam,
                new[] { new Challenge<string[]>(new object[] { "test" }, new string[] { "a" }), });
            var r = a.GetResults(
                new CompileResult(
                    o => Task.FromResult<IReadOnlyList<Option<object, string>>>(
                        new[] { Option.Some<object, string>(null) }))).Result;
            r.Single().Error.HasValue.Should().BeTrue();
        }
    }
}