using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;

namespace CodeGolf.Service
{
    using System;

    using OneOf;

    using Optional;

    public interface ICodeGolfService
    {
        Task<OneOf<int, IReadOnlyList<ChallengeResult>, ErrorSet>> Score(string code, IChallengeSet challenge, CancellationToken cancellationToken);

        IChallengeSet GetDemoChallenge();

        string WrapCode(string code, CancellationToken cancellationToken);

        string DebugCode(string code, CancellationToken cancellationToken);

        Option<ErrorSet> TryCompile(Guid challengeId, string code, in CancellationToken cancellationToken);
    }
}