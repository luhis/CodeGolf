namespace CodeGolf.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Service.Dtos;

    public interface IAdminService
    {
        Task ResetGame();

        Task CreateGame(GameDto challenges, string accessKey, User user, CancellationToken cancellationToken);

        Task<IReadOnlyList<IChallengeSet>> GetAllChallenges(CancellationToken cancellationToken);

        Task<IReadOnlyList<Game>> GetAllGames(CancellationToken cancellationToken);
    }
}
