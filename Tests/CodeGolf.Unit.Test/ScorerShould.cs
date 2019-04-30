using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class ScorerShould
    {
        private readonly IScorer scorer = new Scorer();

        [Fact]
        public void ReturnCorrectScore()
        {
            var r = this.scorer.Score("public string Main(string s){ return s;}");
            r.Should().Be(40);
        }
    }
}