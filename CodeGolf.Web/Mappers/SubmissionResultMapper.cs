namespace CodeGolf.Web.Mappers
{
    using System.Collections.Generic;

    using CodeGolf.Domain;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Web.Models;

    using OneOf;

    public static class SubmissionResultMapper
    {
        public static SubmissionResult Map(
            OneOf<int, IReadOnlyList<ChallengeResult>, IReadOnlyList<CompileErrorMessage>> o) =>
            o.Match(
                score => new SubmissionResult(score),
                runErrors => new SubmissionResult(runErrors),
                compileErrors => new SubmissionResult(compileErrors));
    }
}
