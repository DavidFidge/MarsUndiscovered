using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Quads;
using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Graphics
{
    public class Assets : IAssets
    {
        private const int TileQuadSize = 1;

        public Texture2D TitleTexture { get; set; }
        public SpriteFont MapFont { get; set; }
        public MaterialQuadTemplate WallBackgroundQuad { get; set; }
        public TexturedQuadTemplate WallForegroundQuad { get; set; }
        public TexturedQuadTemplate FloorQuad { get; set; }
        public Effect TextureMaterialEffect { get; set; }

        private readonly IGameProvider _gameProvider;

        public Assets(IGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public void LoadContent()
        {
            TextureMaterialEffect = _gameProvider.Game.Content.Load<Effect>("Effects/TextureMaterial");

            TitleTexture = _gameProvider.Game.Content.Load<Texture2D>("images/title");
            MapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/MapFont");

            WallBackgroundQuad = new MaterialQuadTemplate(_gameProvider);
            WallBackgroundQuad.LoadContent(TileQuadSize, TileQuadSize, new Color(0xFF244BB6));

            WallForegroundQuad = CreateAssetForCharacter('#');
            FloorQuad = CreateAssetForCharacter('·');
        }

        private TexturedQuadTemplate CreateAssetForCharacter(char character)
        {
            var renderTarget = new RenderTarget2D(
                _gameProvider.Game.GraphicsDevice,
                32,
                32,
                false,
                _gameProvider.Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                _gameProvider.Game.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                0,
                RenderTargetUsage.PreserveContents
            );

            var spriteBatch = new SpriteBatch(_gameProvider.Game.GraphicsDevice);

            _gameProvider.Game.GraphicsDevice.SetRenderTarget(renderTarget);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);

            spriteBatch.DrawString(MapFont, character.ToString(), Vector2.Zero, Color.White);

            spriteBatch.End();

            _gameProvider.Game.GraphicsDevice.SetRenderTarget(null);

            var texturedQuad = new TexturedQuadTemplate(_gameProvider);
            texturedQuad.LoadContent(TileQuadSize, TileQuadSize, renderTarget, TextureMaterialEffect);
            texturedQuad.Colour = Color.White;

            return texturedQuad;
        }
    }
}
