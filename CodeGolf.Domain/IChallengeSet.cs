using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;

namespace CodeGolf.Domain
{
    public interface IChallengeSet
    {
        Task<IReadOnlyList<Tuple<Option<IReadOnlyList<string>>, IChallenge>>> GetResults(Func<IChallenge, Task<Option<object, ErrorSet>>> t);

        IReadOnlyList<Type> Params { get; }

        Type ReturnType { get; }

        string Title { get; }

        string Description { get; }

        IReadOnlyList<IChallenge> Challenges { get; }
    }
}