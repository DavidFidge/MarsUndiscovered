using MarsUndiscovered.Interfaces;

using DavidFidge.MonoGame.Core.Graphics.Quads;
using DavidFidge.MonoGame.Core.Interfaces.Components;

using Microsoft.Xna.Framework;

using IDrawable = DavidFidge.MonoGame.Core.Graphics.IDrawable;

namespace MarsUndiscovered.Graphics.Models
{
    public class SelectionModel : IDrawable
    {
        private readonly IGameProvider _gameProvider;
        private readonly IAssetProvider _assetProvider;
        private TexturedQuadTemplate _selectionQuad;
        private Color _selectionColour;

        public Color SelectionColour
        {
            get => _selectionColour;
            set
            {
                _selectionColour = value;
                _selectionQuad.Effect.Parameters["Colour"].SetValue(_selectionColour.ToVector4());
            }
        }

        public SelectionModel(IGameProvider gameProvider, IAssetProvider assetProvider)
        {
            _gameProvider = gameProvider;
            _assetProvider = assetProvider;
        }

        public void LoadContent()
        {
            _selectionQuad = new TexturedQuadTemplate(_gameProvider);

            _selectionQuad.LoadContent(
                1f,
                1f,
                _assetProvider.SelectionTexture,
                _assetProvider.SelectionEffect
                );
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            _selectionQuad.Draw(view, projection, world);
        }
    }
}