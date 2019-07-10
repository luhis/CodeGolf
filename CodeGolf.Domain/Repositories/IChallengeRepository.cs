namespace CodeGolf.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using CodeGolf.Domain.ChallengeInterfaces;

    using Optional;

    public interface IChallengeRepository
    {
        Option<IChallengeSet> GetById(Guid id);

        IReadOnlyList<IChallengeSet> GetAll();
    }
}