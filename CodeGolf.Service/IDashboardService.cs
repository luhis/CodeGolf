namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Service.Dtos;

    using Optional;

    public interface IDashboardService
    {
        Task<Option<HoleDto>> GetCurrentHole(CancellationToken cancellationToken);

        Task<Option<Guid>> NextHole(CancellationToken cancellationToken);

        Task EndHole(Guid holeId);

        Task<Option<AttemptCodeDto>> GetAttemptById(Guid id, CancellationToken cancellationToken);

        Task<Option<IReadOnlyList<AttemptDto>>> GetAttempts(CancellationToken cancellationToken);
    }
}