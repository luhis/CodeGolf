using System;
using System.Collections.Generic;
using CodeGolf.Domain;

namespace CodeGolf.Service.Dtos
{
    public class HoleDto
    {
        public HoleDto(Guid roundId, ChallengeSet<string> challengeSet, TimeSpan duration, IReadOnlyList<Attempt> attempts)
        {
            this.RoundId = roundId;
            this.ChallengeSet = challengeSet;
            this.Duration = duration;
            this.Attempts = attempts;
        }

        public Guid RoundId { get; }

        public ChallengeSet<string> ChallengeSet { get; }

        public TimeSpan Duration { get; }

        public IReadOnlyList<Attempt> Attempts { get; }
    }
}