namespace CodeGolf.Domain
{
    using System;
    using EnsureThat;

    public class Hole
    {
        public Hole(Guid holeId, Guid challengeId, TimeSpan duration)
        {
            this.HoleId = EnsureArg.IsNotEmpty(holeId, nameof(holeId));
            this.ChallengeId = EnsureArg.IsNotEmpty(challengeId, nameof(challengeId));
            this.Duration = duration;
        }

        public Guid HoleId { get; }

        public Guid ChallengeId { get; }

        public TimeSpan Duration { get; }
    }
}