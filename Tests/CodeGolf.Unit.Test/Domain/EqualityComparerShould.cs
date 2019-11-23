namespace CodeGolf.Unit.Test.Domain
{
    using CodeGolf.Domain;
    using FluentAssertions;
    using Xunit;

    public class EqualityComparerShould
    {
        [Fact]
        public void DealWithDifferentLineEndings()
        {
            EqualityComparer.Equal("aaa\nbbb", "aaa\r\nbbb").Should().BeTrue();
        }
    }
}
