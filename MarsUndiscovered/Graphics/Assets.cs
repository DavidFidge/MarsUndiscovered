using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Graphics
{
    public class Assets : IAssets
    {
        private readonly IGameProvider _gameProvider;

        public Assets(IGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public void LoadContent()
        {
        }
    }
}
