using System;
using System.Threading.Tasks;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface IGameService
    {
        Option<Game> GetGame();

        Task<Option<Round>> GetCurrent();

        Task<Option<int, ErrorSet>> Attempt(Guid userId, Guid gameSlotId, string code, ChallengeSet<string> challengeSet);
    }
}