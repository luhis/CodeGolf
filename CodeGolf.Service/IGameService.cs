using System;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface IGameService
    {
        Option<Game> GetGame();

        Task<Option<RoundDto>> GetCurrentHole();

        Task<Option<int, ErrorSet>> Attempt(Guid userId, Guid gameSlotId, string code, ChallengeSet<string> challengeSet);

        Task NextRound();
    }
}