﻿using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Commands
{
    public abstract class BaseMarsGameActionCommand<T> : BaseStatefulGameActionCommand<T>
    {
        public IGameWorld GameWorld { get; private set; }

        public void SetGameWorld(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }
    }
}