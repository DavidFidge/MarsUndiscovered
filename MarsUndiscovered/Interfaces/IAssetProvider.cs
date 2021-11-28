using System;
using System.Collections.Generic;

using MarsUndiscovered.Graphics.Models;

using FrigidRogue.MonoGame.Core.Graphics.Models;

using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Interfaces
{
    public interface IAssetProvider
    {
        Effect SelectionEffect { get; }
        Texture2D SelectionTexture { get; }
        Texture2D GrassTexture { get; }
        Texture2D WoodTexture { get; }
        SelectionModel SelectionModel { get; }
        GameModel MarsUndiscoveredModel { get; }
        void LoadContent();
    }
}