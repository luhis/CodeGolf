using System.Collections.Generic;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Game
    {
        public Game(IReadOnlyList<Hole> holes)
        {
            this.Holes = EnsureArg.IsNotNull(holes, nameof(holes));
        }

        public IReadOnlyList<Hole> Holes { get; }
    }
}