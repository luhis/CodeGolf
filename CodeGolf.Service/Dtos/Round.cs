using System;
using System.Collections.Generic;
using CodeGolf.Domain;
using EnsureThat;

namespace CodeGolf.Service.Dtos
{
    public class Round
    {
        public Round(Guid roundId, ChallengeSet<string> challengeSet, TimeSpan duration,
            IReadOnlyList<Attempt> attempts)
        {
            this.RoundId = EnsureArg.IsNotEmpty(roundId, nameof(roundId));
            this.ChallengeSet = EnsureArg.IsNotNull(challengeSet, nameof(challengeSet));
            this.Duration = duration;
            this.Attempts = EnsureArg.IsNotNull(attempts, nameof(attempts));
        }

        public Guid RoundId { get; }

        public ChallengeSet<string> ChallengeSet { get; }

        public TimeSpan Duration { get; }

        public IReadOnlyList<Attempt> Attempts { get; }
    }
}