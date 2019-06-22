namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.Service.Dtos;

    using Optional;

    public interface IDashboardService
    {
        Task<Option<HoleDto>> GetCurrentHole(CancellationToken cancellationToken);

        Task<Option<Guid>> NextHole();

        Task EndHole(Guid holeId);

        Task<IReadOnlyList<Hole>> GetAllHoles(CancellationToken cancellationToken);

        Task ResetGame();

        Task<Attempt> GetAttemptById(Guid id, CancellationToken cancellationToken);

        Task<Option<IReadOnlyList<AttemptDto>>> GetAttempts(CancellationToken cancellationToken);

        Task<IReadOnlyList<ResultDto>> GetFinalScores(CancellationToken cancellationToken);
    }
}