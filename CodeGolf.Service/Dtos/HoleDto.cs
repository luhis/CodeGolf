using System;
using CodeGolf.Domain;
using EnsureThat;

namespace CodeGolf.Service.Dtos
{
    public class HoleDto
    {
        public HoleDto(Hole hole, DateTime start, DateTime end, DateTime? closedAt, bool hasNext)
        {
            this.Hole = EnsureArg.IsNotNull(hole, nameof(hole));
            this.Start = start;
            this.End = end;
            this.ClosedAt = closedAt;
            this.HasNext = hasNext;
        }

        public Hole Hole { get; }

        public DateTime Start { get; }

        public DateTime End { get; }

        public DateTime? ClosedAt { get; }

        public bool HasNext { get; }
    }
}