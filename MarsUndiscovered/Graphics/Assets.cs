using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Graphics
{
    public class Assets : IAssets
    {
        public const float TileQuadHeight = 1f;
        public const float TileQuadWidth = 0.55f;

        public Texture2D TitleTexture { get; set; }
        public SpriteFont MapFont { get; set; }
        public Effect TextureMaterialEffect { get; set; }
        public MapTileQuad Wall { get; set; }
        public MapTileQuad Floor { get; set; }
        public MapTileQuad Player { get; set; }

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

            Wall = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '#',
                Color.White,
                new Color(0xFF244BB6)
            );

            Floor = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '.',
                Color.Tan
            );

            Player = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '@',
                Color.Yellow
            );
        }
    }
}
