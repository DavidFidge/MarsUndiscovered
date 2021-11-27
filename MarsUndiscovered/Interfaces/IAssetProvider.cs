using System;
using System.Collections.Generic;

using Augmented.Graphics.Models;

using DavidFidge.MonoGame.Core.Graphics.Models;

using Microsoft.Xna.Framework.Graphics;

namespace Augmented.Interfaces
{
    public interface IAssetProvider
    {
        Effect SelectionEffect { get; }
        Texture2D SelectionTexture { get; }
        Texture2D GrassTexture { get; }
        Texture2D WoodTexture { get; }
        SelectionModel SelectionModel { get; }
        GameModel AugmentedModel { get; }
        void LoadContent();
    }
}