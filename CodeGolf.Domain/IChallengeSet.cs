using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;

namespace CodeGolf.Domain
{
    public interface IChallengeSet
    {
        Task<IReadOnlyList<ChallengeResult>> GetResults(Func<object[], Task<Option<OneOf.OneOf<object, object[]>, string>>> t);

        IReadOnlyList<Type> Params { get; }

        Type ReturnType { get; }

        string Title { get; }

        string Description { get; }

        IReadOnlyList<IChallenge> Challenges { get; }
    }
}