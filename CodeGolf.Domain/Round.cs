using System;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Round
    {
        public Round(Guid roundId, ChallengeSet<string> challengeSet, TimeSpan duration)
        {
            this.RoundId = EnsureArg.IsNotEmpty(roundId, nameof(roundId));
            this.ChallengeSet = EnsureArg.IsNotNull(challengeSet, nameof(challengeSet));
            this.Duration = duration;
        }

        public Guid RoundId { get; }

        public ChallengeSet<string> ChallengeSet { get; }

        public TimeSpan Duration { get; }
    }
}