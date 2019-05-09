using System;
using System.Collections.Generic;
using EnsureThat;

namespace CodeGolf.Dtos
{
    public class GameSlot
    {
        public GameSlot(ChallengeSet<string> challengeSet, TimeSpan duration, IReadOnlyList<Attempt> attempts)
        {
            this.ChallengeSet = EnsureArg.IsNotNull(challengeSet, nameof(challengeSet));
            this.Duration = duration;
            this.Attempts = EnsureArg.IsNotNull(attempts, nameof(attempts));
        }

        public ChallengeSet<string> ChallengeSet { get; }

        public TimeSpan Duration { get; }

        public IReadOnlyList<Attempt> Attempts { get; }
    }
}