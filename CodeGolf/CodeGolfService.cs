using System.Collections.Generic;
using System.Linq;
using CodeGolf.Dtos;
using Optional;

namespace CodeGolf
{
    public class CodeGolfService : ICodeGolfService
    {
        private readonly IRunner runner = new Runner();
        private readonly IScorer scorer = new Scorer();

        public Option<int, IReadOnlyList<string>> Score<T>(string code, ChallengeSet<T> challenge)
        {
            var compileResult = this.runner.Compile<T>(code, challenge.Params);
            return compileResult.FlatMap(compiled =>
            {
                var fails = challenge.Challenges.Where(a => compiled(a.Args).Match(success => !success.Equals(a.ExpectedResult), _ => false));
                if (fails.Any())
                {
                    // todo deal with multiple failures
                    return Option.None<int, IReadOnlyList<string>>(new List<string>()
                        {$"Return value incorrect. Expected {fails.First().ExpectedResult}"});
                }
                else
                {
                    return Option.Some<int, IReadOnlyList<string>>(this.scorer.Score(code));
                }
            });
        }
    }
}