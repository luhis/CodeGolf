using System;

namespace CodeGolf.Dtos
{
    public class GameSlot
    {
        public GameSlot(ChallengeSet<string> challengeSet, TimeSpan duration)
        {
            this.ChallengeSet = challengeSet;
            this.Duration = duration;
        }

        public ChallengeSet<string> ChallengeSet { get; }

        public TimeSpan Duration { get; }
    }
}