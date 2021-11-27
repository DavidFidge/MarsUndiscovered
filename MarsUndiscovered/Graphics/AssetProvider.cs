using Augmented.Graphics.Models;
using Augmented.Interfaces;

using DavidFidge.MonoGame.Core.Graphics.Models;
using DavidFidge.MonoGame.Core.Interfaces.Components;

using Microsoft.Xna.Framework.Graphics;

namespace Augmented.Graphics
{
    public class AssetProvider : IAssetProvider
    {
        private readonly IGameProvider _gameProvider;
        public Effect SelectionEffect { get; private set; }
        public Texture2D SelectionTexture { get; private set; }
        public Texture2D GrassTexture { get; private set; }
        public Texture2D WoodTexture { get; private set; }
        public SelectionModel SelectionModel { get; private set; }
        public GameModel AugmentedModel { get; private set; }

        public AssetProvider(IGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public void LoadContent()
        {
            WoodTexture = _gameProvider.Game.Content.Load<Texture2D>(@"Terrain\Wood");
            GrassTexture = _gameProvider.Game.Content.Load<Texture2D>(@"Terrain\Grass");
            SelectionTexture = _gameProvider.Game.Content.Load<Texture2D>(@"Sprites\Selection");

            SelectionEffect = _gameProvider.Game.Content.Load<Effect>(@"Effects\Selection");

            AugmentedModel = new GameModel(@"Models\Augmented", _gameProvider);
            AugmentedModel.LoadContent();

            SelectionModel = new SelectionModel(_gameProvider, this);
            SelectionModel.LoadContent();
        }
    }
}
