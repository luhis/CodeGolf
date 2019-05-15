using System.Collections.Generic;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Game
    {
        public Game(IReadOnlyList<Hole> slots)
        {
            this.Holes = EnsureArg.IsNotNull(slots, nameof(slots));
        }

        public  IReadOnlyList<Hole> Holes { get; }
    }
}