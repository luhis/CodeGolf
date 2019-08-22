namespace CodeGolf.Domain
{
    using System;
    using EnsureThat;

    public class Hole
    {
        public Hole(Guid holeId, Guid challengeId, TimeSpan duration, int rank, DateTime? start, DateTime? end)
        {
            this.HoleId = EnsureArg.IsNotEmpty(holeId, nameof(holeId));
            this.ChallengeId = EnsureArg.IsNotEmpty(challengeId, nameof(challengeId));
            this.Duration = EnsureArg.IsNotDefault(duration, nameof(duration));
            this.Rank = rank;
            this.Start = start;
            this.End = end;
        }

        public Guid HoleId { get; private set; }

        public Guid GameId { get; private set; }

        public Guid ChallengeId { get; private set; }

        public TimeSpan Duration { get; private set; }

        public int Rank { get; private set; }

        public DateTime? Start { get; private set; }

        public DateTime? End { get; private set; }

        public void SetStart(DateTime start) => this.Start = start;

        public void SetEnd(DateTime end) => this.End = end;
    }
}
