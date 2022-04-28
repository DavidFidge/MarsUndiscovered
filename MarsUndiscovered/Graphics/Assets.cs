using System.Collections.Generic;

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
        public SpriteFont GoalMapFont { get; set; }
        public Effect TextureMaterialEffect { get; set; }
        public MapTileQuad MouseHover { get; set; }
        public MapTileQuad Wall { get; set; }
        public MapTileQuad Floor { get; set; }
        public MapTileQuad Player { get; set; }
        public MapTileQuad MapExitDown { get; set; }
        public MapTileQuad MapExitUp { get; set; }
        public MapTileQuad Roach { get; set; }
        public MapTileQuad TeslaCoil { get; set; }
        public MapTileQuad RepairDrone { get; set; }
        public MapTileQuad Weapon { get; set; }
        public MapTileQuad Gadget { get; set; }
        public MapTileQuad NanoFlask { get; set; }
        public MapTileQuad Lightning { get; set; }
        public MapTileQuad ShipRepairParts { get; set; }
        public MapTileQuad FieldOfViewUnrevealedQuad { get; set; }
        public MapTileQuad FieldOfViewHasBeenSeenQuad { get; set; }
        public GoalMapQuad GoalMapQuad { get; set; }
        public IDictionary<char, MapTileQuad> ShipParts { get; set; }

        private readonly IGameProvider _gameProvider;

        private Color _itemColour = Color.Yellow;

        public Assets(IGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public void LoadContent()
        {
            TextureMaterialEffect = _gameProvider.Game.Content.Load<Effect>("Effects/TextureMaterial");

            TitleTexture = _gameProvider.Game.Content.Load<Texture2D>("images/title");
            MapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/MapFont");
            GoalMapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/GoalMapFont");

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

            Weapon = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '↑',
                _itemColour
            );

            Gadget = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '⏻',
                _itemColour
            );

            NanoFlask = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '૪',
                _itemColour
            );

            Roach = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                'r',
                Color.SaddleBrown
            );

            TeslaCoil = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '§',
                Color.SteelBlue,
                Color.White
            );
            
            RepairDrone = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                'd',
                Color.Gray
            );            

            MapExitDown = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '>',
                _itemColour,
                Color.SaddleBrown
            );

            MapExitUp = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '<',
                _itemColour,
                Color.SaddleBrown
            );

            ShipRepairParts = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                MapFont,
                TextureMaterialEffect,
                '&',
                _itemColour
            );

            ShipParts = new Dictionary<char, MapTileQuad>();
            var shipPartChars = "{_-`.+|( ";

            foreach (char ch in shipPartChars)
            {
                var shipPart = new MapTileQuad(
                    _gameProvider,
                    TileQuadWidth,
                    TileQuadHeight,
                    MapFont,
                    TextureMaterialEffect,
                    ch,
                    Color.SteelBlue,
                    Color.Black
                );

                ShipParts.Add(ch, shipPart);
            }

            GoalMapQuad = new GoalMapQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                GoalMapFont,
                TextureMaterialEffect,
                Color.White
            );

            FieldOfViewUnrevealedQuad = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                new Color(Color.Black, 1f)
            );

            FieldOfViewHasBeenSeenQuad = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                new Color(Color.Black, 0.5f)
            );

            MouseHover = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                new Color(Color.LightYellow, 0.3f)
            );

            Lightning = new MapTileQuad(
                _gameProvider,
                TileQuadWidth,
                TileQuadHeight,
                new Color(Color.White, 1f)
            );
        }
    }
}
