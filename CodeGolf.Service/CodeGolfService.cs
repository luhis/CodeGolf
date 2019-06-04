using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Persistence.Static;
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

        Task<Option<int, ErrorSet>> ICodeGolfService.Score(string code, IChallengeSet challenge,
            CancellationToken cancellationToken)
        {
            var compileResult = this.runner.Compile(code, challenge.Params, challenge.ReturnType, cancellationToken);
            return compileResult.Match(async compiled =>
            {
                var fails = (await challenge.GetResults(compiled))
                    .Where(this.IsFailure);
                if (fails.Any())
                {
                    return Option.None<int, ErrorSet>(new ErrorSet(fails.SelectMany(a => a.Item1.Match(b => b, () => new List<string>())).ToList()));
                }
                else
                {
                    return Option.Some<int, ErrorSet>(this.scorer.Score(code));
                }
            }, err => Task.FromResult(Option.None<int, ErrorSet>(err)));
        }

        private bool IsFailure(Tuple<Option<IReadOnlyList<string>>, IChallenge> tuple)
        {
            return tuple.Item1.HasValue;
        }

        IChallengeSet ICodeGolfService.GetDemoChallenge()
        {
            return Challenges.HelloWorld;
        }

        string ICodeGolfService.WrapCode(string code, CancellationToken cancellationToken)
            => this.runner.Wrap(code, cancellationToken);

        string ICodeGolfService.DebugCode(string code, CancellationToken cancellationToken)
            => this.runner.DebugCode(code, cancellationToken);
    }
}