using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Dtos;
using Optional;
using Optional.Async;

namespace CodeGolf
{
    public class CodeGolfService : ICodeGolfService
    {
        private readonly IRunner runner = new Runner();
        private readonly IScorer scorer = new Scorer();

        Task<Option<int, ErrorSet>> ICodeGolfService.Score<T>(string code, ChallengeSet<T> challenge)
        {
            var compileResult = this.runner.Compile<T>(code, challenge.Params);
            var x = compileResult.FlatMapAsync(async compiled =>
            {
                var fails = (await Task.WhenAll(challenge.Challenges.Select(async a => (challenge: a, result: await compiled(a.Args))))).Where(IsFailure);
                if (fails.Any())
                {
                    var failStrings = fails.Select(a => $"Return value incorrect. Expected: {a.challenge.ExpectedResult}, Found: {MapToString(a.result)}").ToList();
                    return Option.None<int, ErrorSet>(new ErrorSet(failStrings));
                }
                else
                {
                    return Option.Some<int, ErrorSet>(this.scorer.Score(code));
                }
            });

            return x;
        }

        private static bool IsFailure<T>((Challenge<T> challenge, Option<T, ErrorSet> result) prop) => 
            prop.result.Match(success => !success.Equals(prop.challenge.ExpectedResult), _ => true);


        private static string MapToString<T>(Option<T, ErrorSet> o) =>
            o.Match(some => some.ToString(), error => string.Join("", error));
    }
}