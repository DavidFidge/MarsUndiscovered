﻿using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move DownRight", DefaultKey = Keys.NumPad3)]
    public class MoveDownRightRequest : MoveRequest
    {
        public MoveDownRightRequest() : base(Direction.DownRight)
        {
        }
    }
}