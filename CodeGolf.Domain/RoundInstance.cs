using System;

namespace CodeGolf.Domain
{
    public class RoundInstance
    {
        public RoundInstance(Guid roundId, DateTime start, DateTime end)
        {
            this.RoundId = roundId;
            this.Start = start;
            this.End = end;
        }

        public Guid RoundId { get; }

        public DateTime Start { get; }

        public DateTime End { get; }
    }
}