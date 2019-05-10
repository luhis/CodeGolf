using System;
using CodeGolf.Service.Dtos;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class ChallengeSetShould
    {
        [Fact]
        public void ThrowWhenTheChallengeParamsAreMismatched()
        {
            Action a = () =>
                new ChallengeSet<string>("a", "b", new[] {typeof(string)}, new[] {new Challenge<string>(new object[]{42}, "test")});
            a.Should().Throw<Exception>().WithMessage("Mismatched parameters");
        }

        [Fact]
        public void NotThrowWhenTheChallengeParamsAreNotMismatched()
        {
            var a = new ChallengeSet<string>("a", "b", new[] {typeof(string)},
                new[] {new Challenge<string>(new object[] {"test"}, "test")});
            a.Should().NotBeNull();
        }
    }
}