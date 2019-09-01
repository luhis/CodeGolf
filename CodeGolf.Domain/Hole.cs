namespace CodeGolf.Domain
{
    using System;
    using EnsureThat;

    public class Hole
    {
        public Hole(Guid holeId, Guid challengeId, TimeSpan duration, int rank)
        {
            this.HoleId = EnsureArg.IsNotEmpty(holeId, nameof(holeId));
            this.ChallengeId = EnsureArg.IsNotEmpty(challengeId, nameof(challengeId));
            this.Duration = duration;
            this.Rank = rank;
        }

        public Guid HoleId { get; private set; }

        public Guid ChallengeId { get; private set; }

        public TimeSpan Duration { get; private set; }

        public int Rank { get; private set; }
    }
}
