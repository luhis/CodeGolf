﻿namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Service.Dtos;
    using OneOf;
    using Optional;

    public interface IGameService
    {
        Task<Option<HoleDto>> GetCurrentHole(CancellationToken cancellationToken);

        Task<OneOf<int, IReadOnlyList<ChallengeResult>, IReadOnlyList<CompileErrorMessage>>> Attempt(
            User user,
            Guid holeId,
            string code,
            IChallengeSet challengeSet,
            CancellationToken cancellationToken);
    }
}
