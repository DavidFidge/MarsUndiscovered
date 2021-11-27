using MarsUndiscovered.Graphics.Models;
using MarsUndiscovered.Interfaces;

using DavidFidge.MonoGame.Core.Graphics.Models;
using DavidFidge.MonoGame.Core.Interfaces.Components;

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
