using System.Collections.Generic;
using EnsureThat;

namespace CodeGolf.Dtos
{
    public class ChallengeSet<T>
    {
        public ChallengeSet(IReadOnlyList<Challenge<T>> challenges)
        {
            this.Challenges = EnsureArg.IsNotNull(challenges, nameof(challenges));
        }

        public IReadOnlyList<Challenge<T>> Challenges { get; }
    }
}