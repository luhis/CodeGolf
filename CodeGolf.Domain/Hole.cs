using System;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Hole
    {
        public Hole(Guid holeId, IChallengeSet challengeSet, TimeSpan duration)
        {
            this.HoleId = EnsureArg.IsNotEmpty(holeId, nameof(holeId));
            this.ChallengeSet = EnsureArg.IsNotNull(challengeSet, nameof(challengeSet));
            this.Duration = duration;
        }

        public Guid HoleId { get; }

        public IChallengeSet ChallengeSet { get; }

        public TimeSpan Duration { get; }
    }
}