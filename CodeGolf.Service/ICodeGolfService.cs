namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Service.Dtos;
    using OneOf;
    using Optional;

    public interface ICodeGolfService
    {
        Task<OneOf<int, IReadOnlyList<ChallengeResult>, IReadOnlyList<CompileErrorMessage>>> Score(string code, IChallengeSet challenge, CancellationToken cancellationToken);

        IChallengeSet GetDemoChallenge();

        string WrapCode(string code, CancellationToken cancellationToken);

        string DebugCode(string code, CancellationToken cancellationToken);

        Option<IReadOnlyList<CompileErrorMessage>> TryCompile(Guid challengeId, string code, CancellationToken cancellationToken);
    }
}
