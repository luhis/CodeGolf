using CodeGolf.Dtos;
using Optional;

namespace CodeGolf
{
    public interface ICodeGolfService
    {
        Option<int, ErrorSet> Score<T>(string code, ChallengeSet<T> challenge);
    }
}