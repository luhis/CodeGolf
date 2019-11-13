namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Persistence.Static;
    using CodeGolf.Service.Dtos;
    using EnsureThat;
    using OneOf;
    using Optional;

    public class CodeGolfService : ICodeGolfService
    {
        private readonly IRunner runner;

        private readonly IScorer scorer;

        public CodeGolfService(IRunner runner, IScorer scorer)
        {
            this.runner = runner;
            this.scorer = scorer;
        }

        Task<OneOf<int, IReadOnlyList<ChallengeResult>, IReadOnlyList<CompileErrorMessage>>> ICodeGolfService.Score(
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
                        if (results.Any(a => a.Error != null))
                        {
                            return results.ToList();
                        }
                        else
                        {
                            return this.scorer.Score(code);
                        }
                    },
                err => Task.FromResult((OneOf<int, IReadOnlyList<ChallengeResult>, IReadOnlyList<CompileErrorMessage>>)err.ToArray()));
        }

        IChallengeSet ICodeGolfService.GetDemoChallenge()
        {
            return Challenges.RocketScience;
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

        Option<IReadOnlyList<CompileErrorMessage>> ICodeGolfService.TryCompile(string code, CancellationToken cancellationToken)
        {
            return this.runner.TryCompile(
                code,
                cancellationToken);
        }
    }
}
