namespace CodeGolf.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;

    public interface IAdminService
    {
        Task ResetGame();

        Task<IReadOnlyList<Hole>> GetAllHoles(CancellationToken cancellationToken);
    }
}