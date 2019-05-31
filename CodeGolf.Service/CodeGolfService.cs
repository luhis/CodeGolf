using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Persistence.Static;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public class CodeGolfService : ICodeGolfService
    {
        private readonly IRunner runner;
        private readonly IScorer scorer;

        public CodeGolfService(IRunner runner, IScorer scorer)
        {
            this.runner = runner;
            this.scorer = scorer;
        }

        Task<Option<int, ErrorSet>> ICodeGolfService.Score<T>(string code, ChallengeSet<T> challenge, CancellationToken cancellationToken)
        {
            var compileResult = this.runner.Compile<T>(code, challenge.Params, cancellationToken);
            return compileResult.Match(async compiled =>
            {
                var fails = (await Task.WhenAll(challenge.Challenges.Select(async a => (challenge: a, result: await compiled(a.Args).ConfigureAwait(false)))).ConfigureAwait(false)).Where(IsFailure);
                if (fails.Any())
                {
                    var failStrings = fails.Select(a => $"Return value incorrect. Expected: {a.challenge.ExpectedResult}, Found: {MapToString(a.result)}").ToList();
                    return Option.None<int, ErrorSet>(new ErrorSet(failStrings));
                }
                else
                {
                    return Option.Some<int, ErrorSet>(this.scorer.Score(code));
                }
            }, err => Task.FromResult(Option.None<int, ErrorSet>(err)));
        }

        ChallengeSet<string> ICodeGolfService.GetDemoChallenge()
        {
            return Challenges.HelloWorld;
        }

        string ICodeGolfService.WrapCode(string code, CancellationToken cancellationToken)
            => this.runner.Wrap(code, cancellationToken);

        string ICodeGolfService.DebugCode(string code, CancellationToken cancellationToken)
            => this.runner.DebugCode(code, cancellationToken);

        private static bool IsFailure<T>((Challenge<T> challenge, Option<T, ErrorSet> result) prop) => 
            prop.result.Match(success => !success.Equals(prop.challenge.ExpectedResult), _ => true);

        private static string MapToString<T>(Option<T, ErrorSet> o) =>
            o.Match(some => some.ToString(), error => string.Join("", error.Errors));
    }
}