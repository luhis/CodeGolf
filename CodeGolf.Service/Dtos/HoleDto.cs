using System;
using System.Collections.Generic;
using CodeGolf.Domain;

namespace CodeGolf.Service.Dtos
{
    public class HoleDto
    {
        public HoleDto(Hole hole, DateTime start, DateTime end, IReadOnlyList<Attempt> attempts)
        {
            this.Hole = hole;
            this.Start = start;
            this.End = end;
            this.Attempts = attempts;
        }

        public Hole Hole { get; }

        public DateTime Start { get; }

        public DateTime End { get; }

        public IReadOnlyList<Attempt> Attempts { get; }
    }
}