using System;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface IGameService
    {
        Task<Option<HoleDto>> GetCurrentHole();

        Task<Option<int, ErrorSet>> Attempt(string userId, Guid holeId, string code, ChallengeSet<string> challengeSet, CancellationToken cancellationToken);

        Task NextRound();

        Task ResetGame();
    }
}