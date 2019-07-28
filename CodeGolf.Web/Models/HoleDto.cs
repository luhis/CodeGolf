using System;
using CodeGolf.Domain;
using EnsureThat;

namespace CodeGolf.Web.Models
{
    public class HoleDto
    {
        public HoleDto(Hole hole, DateTime start, DateTime end, DateTime? closedAt, bool hasNext, ChallengeSetDto challengeSet)
        {
            this.Hole = EnsureArg.IsNotNull(hole, nameof(hole));
            this.Start = start;
            this.End = end;
            this.ClosedAt = closedAt;
            this.HasNext = hasNext;
            this.ChallengeSet = EnsureArg.IsNotNull(challengeSet, nameof(challengeSet));
        }

        public Hole Hole { get; }

        public DateTime Start { get; }

        public DateTime End { get; }

        public DateTime? ClosedAt { get; }

        public bool HasNext { get; }

        public ChallengeSetDto ChallengeSet { get; }
    }
}