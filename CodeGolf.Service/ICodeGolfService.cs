using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface ICodeGolfService
    {
        Task<Option<int, ErrorSet>> Score<T>(string code, ChallengeSet<T> challenge);

        ChallengeSet<string> GetDemoChallenge();
    }
}