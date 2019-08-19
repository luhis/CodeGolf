namespace CodeGolf.Domain
{
    using System.Collections.Generic;
    using EnsureThat;

    public class Game
    {
        public Game(IReadOnlyList<Hole> holes)
        {
            this.Holes = EnsureArg.IsNotNull(holes, nameof(holes));
        }

        public IReadOnlyList<Hole> Holes { get; private set; }
    }
}