﻿using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Up", DefaultKey = Keys.NumPad8)]
    public class MoveUpRequest : MoveRequest
    {
        public MoveUpRequest() : base(Direction.Up)
        {
        }
    }
}