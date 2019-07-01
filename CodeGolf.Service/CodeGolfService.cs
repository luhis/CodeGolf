using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;
using CodeGolf.Persistence.Static;
using EnsureThat;

namespace CodeGolf.Service
{
    using OneOf;

    public class CodeGolfService : ICodeGolfService
    {
        private readonly IRunner runner;

        private readonly IScorer scorer;

        public CodeGolfService(IRunner runner, IScorer scorer)
        {
            this.runner = runner;
            this.scorer = scorer;
        }

        Task<OneOf<int, IReadOnlyList<ChallengeResult>, ErrorSet>> ICodeGolfService.Score(
            string code,
            IChallengeSet challenge,
            CancellationToken cancellationToken)
        {
            var compileResult = this.runner.Compile(
                code,
                challenge.Params.Select(a => a.Type).ToArray(),
                challenge.ReturnType,
                cancellationToken);
            return compileResult.Match(
                async compiled =>
                    {
                        var results = await challenge.GetResults(compiled);
                        if (results.Any(a => a.Error.HasValue))
                        {
                            return (OneOf<int, IReadOnlyList<ChallengeResult>, ErrorSet>)results.ToList();
                        }
                        else
                        {
                            return (OneOf<int, IReadOnlyList<ChallengeResult>, ErrorSet>)this.scorer.Score(code);
                        }
                    },
                err => Task.FromResult((OneOf<int, IReadOnlyList<ChallengeResult>, ErrorSet>)err));
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
