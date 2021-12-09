using FrigidRogue.MonoGame.Core.View;

using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Views;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class TitleScreen : Screen
    {
        public IAssets Assets { get; set; }

        private SpriteBatch _spriteBatch;

        public TitleScreen(TitleView titleView) : base(titleView)
        {
        }

        protected override void InitializeInternal()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(Assets.TitleTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.End();

            base.Draw();
        }
    }
}