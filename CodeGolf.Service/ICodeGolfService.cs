using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using Optional;

namespace CodeGolf.Service
{
    public interface ICodeGolfService
    {
        Task<Option<int, ErrorSet>> Score(string code, IChallengeSet challenge, CancellationToken cancellationToken);

        IChallengeSet GetDemoChallenge();

        string WrapCode(string code, CancellationToken cancellationToken);

        string DebugCode(string code, CancellationToken cancellationToken);
    }
}