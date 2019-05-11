using System;
using System.Threading.Tasks;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface IGameService
    {
        Option<Game> GetGame();

        Option<GameSlot> GetCurrent();

        Task<Option<int, ErrorSet>> Attempt(Guid userId, string code, ChallengeSet<string> challengeSet);
    }
}