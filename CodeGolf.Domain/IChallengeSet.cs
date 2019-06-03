using System;
using System.Collections.Generic;

namespace CodeGolf.Domain
{
    public interface IChallengeSet
    {
        IReadOnlyList<Tuple<bool, IChallenge>> GetResults(Func<object[], object> t);
    }
}