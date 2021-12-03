using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;

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
