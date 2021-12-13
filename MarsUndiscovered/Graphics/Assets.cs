using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Graphics
{
    public class Assets : IAssets
    {
        private readonly IGameProvider _gameProvider;
        public Texture2D TitleTexture { get; set; }
        public SpriteFont MapFont { get; set; }
        public Texture2D Wall { get; set; }
        public Texture2D Floor { get; set; }

        public Assets(IGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public void LoadContent()
        {
            TitleTexture = _gameProvider.Game.Content.Load<Texture2D>("images/title");
            MapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/MapFont");

            var wallRenderTarget = new RenderTarget2D(_gameProvider.Game.GraphicsDevice,
                640,
                640,
                false,
                _gameProvider.Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                _gameProvider.Game.GraphicsDevice.PresentationParameters.DepthStencilFormat, 0,
                RenderTargetUsage.PreserveContents);

            var spriteBatch = new SpriteBatch(_gameProvider.Game.GraphicsDevice);

            _gameProvider.Game.GraphicsDevice.SetRenderTarget(wallRenderTarget);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            
            spriteBatch.DrawString(MapFont, "###############", Vector2.Zero, Color.White);

            spriteBatch.End();

            _gameProvider.Game.GraphicsDevice.SetRenderTarget(null);

            Wall = wallRenderTarget;
        }
    }
}
