namespace CodeGolf.Web.Models
{
    using System.Collections.Generic;

    using CodeGolf.Domain;
    using CodeGolf.Service.Dtos;

    public class SubmissionResult
    {
        public SubmissionResult(int? score, IReadOnlyList<CompileErrorMessage> compileErrors, IReadOnlyList<ChallengeResult> runErrors)
        {
            this.Score = score;
            this.CompileErrors = compileErrors;
            this.RunErrors = runErrors;
        }

        public int? Score { get; }

        public IReadOnlyList<ChallengeResult> RunErrors { get; }

        public IReadOnlyList<CompileErrorMessage> CompileErrors { get; }
    }
}