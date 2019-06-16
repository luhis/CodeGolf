using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;
using Optional;

namespace CodeGolf.Service
{
    public interface ICodeGolfService
    {
        Task<Option<Option<int, IReadOnlyList<ChallengeResult>>, ErrorSet>> Score(string code, IChallengeSet challenge, CancellationToken cancellationToken);

        IChallengeSet GetDemoChallenge();

        string WrapCode(string code, CancellationToken cancellationToken);

        string DebugCode(string code, CancellationToken cancellationToken);
    }
}