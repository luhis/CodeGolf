using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface ICodeGolfService
    {
        Task<Option<int, ErrorSet>> Score<T>(string code, ChallengeSet<T> challenge, CancellationToken cancellationToken);

        ChallengeSet<string> GetDemoChallenge();

        string WrapCode(string code, CancellationToken cancellationToken);
        string DebugCode(string code, CancellationToken cancellationToken);
    }
}