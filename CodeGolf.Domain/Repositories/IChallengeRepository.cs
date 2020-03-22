namespace CodeGolf.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using CodeGolf.Domain.ChallengeInterfaces;

    using Optional;

    public interface IChallengeRepository
    {
        Option<IChallengeSet> GetById(Guid id, CancellationToken cancellationToken);

        IReadOnlyList<IChallengeSet> GetAll(CancellationToken cancellationToken);
    }
}
