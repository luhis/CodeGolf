using System;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class RunnerShould
    {
        private readonly Runner runner = new Runner();

        [Fact]
        public void ShouldFail()
        {
            this.runner.IsCorrect("throw new Exception();", _ => false).Should().BeFalse();
        }

        [Fact]
        public void ShouldPass()
        {
            this.runner.IsCorrect("throw new Exception();", _ => true).Should().BeTrue();
        }

        [Fact]
        public void ShouldFailToCompileInvalidApplication()
        {
            Action a = () => this.runner.IsCorrect("abc", _ => true);
            a.Should().Throw<Exception>();
        }
    }
}
