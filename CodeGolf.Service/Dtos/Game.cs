﻿using System.Collections.Generic;
using EnsureThat;

namespace CodeGolf.Service.Dtos
{
    public class Game
    {
        public Game(IReadOnlyList<GameSlot> slots)
        {
            this.Slots = EnsureArg.IsNotNull(slots, nameof(slots));
        }

        public  IReadOnlyList<GameSlot> Slots { get; }
    }
}