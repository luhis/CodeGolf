using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Persistence.Static;
using EnsureThat;
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

        Task<Option<Option<int, IReadOnlyList<ChallengeResult>>, ErrorSet>> ICodeGolfService.Score(string code,
            IChallengeSet challenge,
            CancellationToken cancellationToken)
        {
            var compileResult = this.runner.Compile(code, challenge.Params, challenge.ReturnType, cancellationToken);
            return compileResult.Match(async compiled =>
            {
                var fails = (await challenge.GetResults(compiled))
                    .Where(this.IsFailure);
                if (fails.Any())
                {
                    return Option.Some<Option<int, IReadOnlyList<ChallengeResult>>, ErrorSet>(
                        Option.None<int, IReadOnlyList<ChallengeResult>>(
                            fails.ToList()));
                }
                else
                {
                    return Option.Some<Option<int, IReadOnlyList<ChallengeResult>>, ErrorSet>(
                        Option.Some<int, IReadOnlyList<ChallengeResult>>(this.scorer.Score(code)));
                }
            }, err => Task.FromResult(Option.None<Option<int, IReadOnlyList<ChallengeResult>>, ErrorSet>(err)));
        }

        private bool IsFailure(ChallengeResult tuple)
        {
            return tuple.Error.HasValue;
        }

        IChallengeSet ICodeGolfService.GetDemoChallenge()
        {
            return Challenges.FizzBuzz;
        }

        string ICodeGolfService.WrapCode(string code, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(code, nameof(code));
            return this.runner.Wrap(code, cancellationToken);
        }

        string ICodeGolfService.DebugCode(string code, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(code, nameof(code));
            return this.runner.DebugCode(code, cancellationToken);
        }
    }
}
