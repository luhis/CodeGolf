using System;
using System.Collections.Generic;
using CodeGolf.Domain;
using EnsureThat;

namespace CodeGolf.Service.Dtos
{
    public class HoleDto
    {
        public HoleDto(Hole hole, DateTime start, DateTime end, IReadOnlyList<Attempt> attempts)
        {
            this.Hole = EnsureArg.IsNotNull(hole, nameof(hole));
            this.Start = start;
            this.End = end;
            this.Attempts = EnsureArg.IsNotNull(attempts, nameof(attempts));
        }

        public Hole Hole { get; }

        public DateTime Start { get; }

        public DateTime End { get; }

        public IReadOnlyList<Attempt> Attempts { get; }
    }
}