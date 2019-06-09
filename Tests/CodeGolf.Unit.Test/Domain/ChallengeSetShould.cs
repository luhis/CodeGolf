using System;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using FluentAssertions;
using Optional;
using Xunit;

namespace CodeGolf.Unit.Test.Domain
{
    public class ChallengeSetShould
    {
        [Fact]
        public void ThrowWhenTheChallengeParamsAreMismatched()
        {
            Action a = () =>
                new ChallengeSet<string>("a", "b", new[] {typeof(string)},
                    new[] {new Challenge<string>(new object[] {42}, "test")});
            a.Should().Throw<Exception>().WithMessage("Mismatched parameters");
        }

        [Fact]
        public void NotThrowWhenTheChallengeParamsAreNotMismatched()
        {
            var a = new ChallengeSet<string>("a", "b", new[] {typeof(string)},
                new[] {new Challenge<string>(new object[] {"test"}, "test")});
            a.Should().NotBeNull();
        }

        [Fact]
        public void ReturnTrueResultWhenCorrect()
        {
            var a = (IChallengeSet) new ChallengeSet<string>("a", "b", new[] {typeof(string)},
                new[] {new Challenge<string>(new object[] {"test"}, "test")});
            var r = a.GetResults(new CompileResult(o => Task.FromResult(Option.Some<object, string>("test")))).Result;
            r.Single().Error
                .HasValue.Should().BeFalse();
        }

        [Fact]
        public void ReturnFalseResultWhenIncorrect()
        {
            var a = (IChallengeSet)new ChallengeSet<string>("a", "b", new[] { typeof(string) },
                new[] { new Challenge<string>(new object[] { "test" }, "test") });
            var r = a.GetResults(new CompileResult(o => Task.FromResult(Option.Some< object, string >("testXX")))).Result;
            r.Single().Error
                .HasValue.Should().BeTrue();
        }
    }
}