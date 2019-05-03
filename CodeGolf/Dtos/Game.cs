using System.Collections.Generic;

namespace CodeGolf.Dtos
{
    public class Game
    {
        public Game(IReadOnlyList<GameSlot> slots)
        {
            this.Slots = slots;
        }

        public  IReadOnlyList<GameSlot> Slots { get; }
    }
}