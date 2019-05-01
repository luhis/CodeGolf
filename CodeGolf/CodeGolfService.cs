using System.Linq;
using CodeGolf.Dtos;
using Optional;

namespace CodeGolf
{
    public class CodeGolfService : ICodeGolfService
    {
        private readonly IRunner runner = new Runner();
        private readonly IScorer scorer = new Scorer();

        public Option<int, ErrorSet> Score<T>(string code, ChallengeSet<T> challenge)
        {
            var compileResult = this.runner.Compile<T>(code, challenge.Params);
            return compileResult.FlatMap(compiled =>
            {
                var fails = challenge.Challenges.Select(a => (challenge: a, result: compiled(a.Args))).Where(a => a.result.Match(success => !success.Equals(a.challenge.ExpectedResult), _ => false));
                if (fails.Any())
                {
                    var failStrings = fails.Select(a => $"Expected: {a.challenge.ExpectedResult}, Found: {MapToString(a.result)}");
                    return Option.None<int, ErrorSet>(new ErrorSet($"Return value incorrect. Expected {string.Join(", ", failStrings)}"));
                }
                else
                {
                    return Option.Some<int, ErrorSet>(this.scorer.Score(code));
                }
            });
        }

        private static string MapToString<T>(Option<T, ErrorSet> o) =>
            o.Match(some => some.ToString(), error => string.Join("", error));
    }
}