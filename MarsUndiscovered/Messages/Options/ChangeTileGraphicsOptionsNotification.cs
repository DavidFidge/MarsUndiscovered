﻿using FrigidRogue.MonoGame.Core.Components.Mediator;
using MarsUndiscovered.Graphics;

namespace MarsUndiscovered.Messages
{
    public class ChangeTileGraphicsOptionsNotification : INotification
    {
        public TileGraphicOptions TileGraphicOptions { get; }

        public ChangeTileGraphicsOptionsNotification(TileGraphicOptions tileGraphicOptions)
        {
            TileGraphicOptions = tileGraphicOptions;
        }
    }
}