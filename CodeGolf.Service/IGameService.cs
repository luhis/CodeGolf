﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface IGameService
    {
        Task<Option<HoleDto>> GetCurrentHole(CancellationToken cancellationToken);

        Task<Option<Option<int, IReadOnlyList<ChallengeResult>>, ErrorSet>> Attempt(string userId, Guid holeId, string code, IChallengeSet challengeSet, CancellationToken cancellationToken);

        Task NextRound();

        Task ResetGame();

        Task<Option<IReadOnlyList<Attempt>>> GetAttempts(CancellationToken cancellationToken);

        Task<Attempt> GetAttemptById(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Hole>> GetAllHoles(CancellationToken cancellationToken);
    }
}