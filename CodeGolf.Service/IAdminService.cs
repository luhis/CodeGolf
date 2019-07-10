namespace CodeGolf.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain.ChallengeInterfaces;

    public interface IAdminService
    {
        Task ResetGame();

        Task<IReadOnlyList<IChallengeSet>> GetAllHoles(CancellationToken cancellationToken);
    }
}