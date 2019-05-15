using System;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class RoundInstance
    {
        public RoundInstance(Guid holeId, DateTime start, DateTime end)
        {
            this.HoleId = EnsureArg.IsNotEmpty(holeId, nameof(holeId));
            this.Start = start;
            this.End = end;
        }

        public Guid HoleId { get; }

        public DateTime Start { get; }

        public DateTime End { get; }
    }
}