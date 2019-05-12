using System.Collections.Generic;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Game
    {
        public Game(IReadOnlyList<Round> slots)
        {
            this.Rounds = EnsureArg.IsNotNull(slots, nameof(slots));
        }

        public  IReadOnlyList<Round> Rounds { get; }
    }
}