using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Views
{
    public class WaveFunctionCollapseView : BaseMarsUndiscoveredView<WaveFunctionCollapseViewModel, WaveFunctionCollapseData>
    {
        private Panel _tilesPanel;
        private Panel _mapPanel;
        private SpriteBatch _spriteBatch;

        public WaveFunctionCollapseView(WaveFunctionCollapseViewModel waveFunctionCollapseViewModel)
            : base(waveFunctionCollapseViewModel)
        {
            _tilesPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(Constants.LeftPanelWidth)
                .SkinNone()
                .NoPadding()
                .Height(0.999f);

            _mapPanel = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(Constants.MiddlePanelWidth)
                .SkinNone()
                .NoPadding()
                .Height(0.999f);
        }

        protected override void InitializeInternal()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(Assets.TitleTexture, Game.GraphicsDevice.ViewportRectangle(), Color.White);
            _spriteBatch.Draw(
                Assets.TitleTextTexture,
                new Vector2(
                    Game.GraphicsDevice.ViewportRectangle().Width / 2 - Assets.TitleTextTexture.Width / 2,
                    Game.GraphicsDevice.ViewportRectangle().Height / 10f
                    ),
                Color.White);
            _spriteBatch.End();
        }
    }
}