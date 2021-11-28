using MarsUndiscovered.Graphics.Models;
using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Graphics.Models;
using FrigidRogue.MonoGame.Core.Interfaces.Components;

using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Graphics
{
    public class AssetProvider : IAssetProvider
    {
        private readonly IGameProvider _gameProvider;

        public AssetProvider(IGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public void LoadContent()
        {
        }
    }
}
