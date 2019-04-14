using System.Collections.Generic;

namespace CodeGolf.Dtos
{
    public class ChallengeSet<T>
    {
        public ChallengeSet(IReadOnlyList<Challenge<T>> challenges)
        {
            this.Challenges = challenges;
        }

        public IReadOnlyList<Challenge<T>> Challenges { get; }
    }
}