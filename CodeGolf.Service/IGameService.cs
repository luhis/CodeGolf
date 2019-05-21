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

        Task<Option<HoleDto>> GetCurrentHole();

        Task<Option<int, ErrorSet>> Attempt(Guid userId, Guid holeId, string code, ChallengeSet<string> challengeSet);

        Task NextRound();
    }
}