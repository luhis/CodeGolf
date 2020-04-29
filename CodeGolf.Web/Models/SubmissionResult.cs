namespace CodeGolf.Web.Models
{
    using System.Collections.Generic;

    using CodeGolf.Domain;
    using CodeGolf.Service.Dtos;

    public class SubmissionResult
    {
        public SubmissionResult(int? score)
        {
            this.Type = "Score";
            this.Score = score;
            this.CompileErrors = null;
            this.RunErrors = null;
        }

        public SubmissionResult(IReadOnlyList<CompileErrorMessage> compileErrors)
        {
            this.Type = "CompileError";
            this.Score = 0;
            this.CompileErrors = compileErrors;
            this.RunErrors = null;
        }

        public SubmissionResult(IReadOnlyList<ChallengeResult> runErrors)
        {
            this.Type = "RunResultSet";
            this.Score = 0;
            this.CompileErrors = null;
            this.RunErrors = runErrors;
        }

        public string Type { get; }

        public int? Score { get; }

        public IReadOnlyList<ChallengeResult> RunErrors { get; }

        public IReadOnlyList<CompileErrorMessage> CompileErrors { get; }
    }
}
