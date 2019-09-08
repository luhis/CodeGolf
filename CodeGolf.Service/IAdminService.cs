namespace CodeGolf.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;

    public interface IAdminService
    {
        Task ResetGame();

        Task<IReadOnlyList<IChallengeSet>> GetAllChallenges(in CancellationToken cancellationToken);

        Task<IReadOnlyList<Game>> GetAllGames(CancellationToken cancellationToken);
    }
}
