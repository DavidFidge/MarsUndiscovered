using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Graphics
{
    public class Assets : IAssets
    {
        public Texture2D TitleTexture { get; set; }
        public Texture2D TitleTextTexture { get; set; }
        public SpriteFont MapFont { get; set; }
        public SpriteFont GoalMapFont { get; set; }
        public MapTileTexture MouseHover { get; set; }
        public MapTileTexture Wall { get; set; }
        public MapTileTexture Floor { get; set; }
        public MapTileTexture Player { get; set; }
        public MapTileTexture MapExitDown { get; set; }
        public MapTileTexture MapExitUp { get; set; }
        public IDictionary<string, MapTileTexture> Monsters { get; set; }
        public MapTileTexture Weapon { get; set; }
        public MapTileTexture Gadget { get; set; }
        public MapTileTexture NanoFlask { get; set; }
        public MapTileTexture Lightning { get; set; }
        public MapTileTexture LineAttackEastWest { get; set; }
        public MapTileTexture LineAttackNorthEastSouthWest { get; set; }
        public MapTileTexture LineAttackNorthWestSouthEast { get; set; }
        public MapTileTexture LineAttackNorthSouth { get; set; }
        public MapTileTexture ShipRepairParts { get; set; }
        public MapTileTexture FieldOfViewUnrevealedTexture { get; set; }
        public MapTileTexture FieldOfViewHasBeenSeenTexture { get; set; }
        public GoalMapTileTexture GoalMapTileTexture { get; set; }
        public IDictionary<char, MapTileTexture> ShipParts { get; set; }
        public IDictionary<char, MapTileTexture> MiningFacilitySection { get; set; }

        private readonly IGameProvider _gameProvider;

        private Color _itemColour = Color.Yellow;
        private Color _lineAttackColour = Color.LightGray;

        public Assets(IGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public void LoadContent()
        {
            Monsters = new Dictionary<string, MapTileTexture>();

            TitleTexture = _gameProvider.Game.Content.Load<Texture2D>("images/Title");
            TitleTextTexture = _gameProvider.Game.Content.Load<Texture2D>("images/TitleText");
            MapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/MapFont");
            GoalMapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/GoalMapFont");

            Wall = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '#',
                0.999f,
                Color.White,
                new Color(0xFF244BB6)
            );

            Floor = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '.',
                0.998f,
                Color.Tan
            );
            
            MapExitDown = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                
                '>',
                0.997f,
                _itemColour,
                Color.SaddleBrown
            );

            MapExitUp = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '<',
                0.996f,
                _itemColour,
                Color.SaddleBrown
            );
            
            ShipParts = new Dictionary<char, MapTileTexture>();
            var shipPartChars = "{_-`.+|( ";

            foreach (char ch in shipPartChars)
            {
                var shipPartDrawDepth = ch * 0.000001f;
                var shipPart = new MapTileTexture(
                    _gameProvider,
                    Constants.TileWidth,
                    Constants.TileHeight,
                    MapFont,
                    ch,
                    0.8f + shipPartDrawDepth,
                    Color.SteelBlue,
                    Color.Black
                );

                ShipParts.Add(ch, shipPart);
            }
            
            MiningFacilitySection = new Dictionary<char, MapTileTexture>();
            var miningFacilitySectionChars = @"_()|#-`'.+:\/= ";

            foreach (char ch in miningFacilitySectionChars)
            {
                var miningFacilitySectionDrawDepth = ch * 0.001001f;
                var miningFacilitySection = new MapTileTexture(
                    _gameProvider,
                    Constants.TileWidth,
                    Constants.TileHeight,
                    MapFont,
                    ch,
                    0.8f + miningFacilitySectionDrawDepth,
                    Color.MediumPurple,
                    Color.Black
                );

                MiningFacilitySection.Add(ch, miningFacilitySection);
            }
            
            Weapon = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '↑',
                0.598f,
                _itemColour
            );

            Gadget = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '⏻',
                0.597f,
                _itemColour
            );

            NanoFlask = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '૪',
                0.596f,
                _itemColour
            );
            
            ShipRepairParts = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '&',
                0.595f,
                _itemColour
            );
            
            var actorDrawDepth = 0.4999f;
            
            Player = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '@',
                actorDrawDepth,
                Color.Yellow
            );

            actorDrawDepth -= 0.0001f;

            foreach (var breed in Breed.Breeds)
            {
                var mapTileQuad = new MapTileTexture(
                    _gameProvider,
                    Constants.TileWidth,
                    Constants.TileHeight,
                    MapFont,
                        breed.Value.AsciiCharacter,
                    actorDrawDepth,
                    breed.Value.ForegroundColour,
                    breed.Value.BackgroundColour
                );

                Monsters.Add(breed.Key, mapTileQuad);
            }

            GoalMapTileTexture = new GoalMapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                GoalMapFont,
                Color.White
            );

            FieldOfViewUnrevealedTexture = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                new Color(Color.Black, 1f),
                0.199f
            );

            FieldOfViewHasBeenSeenTexture = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                new Color(Color.Black, 0.8f),
                0.198f
            );

            MouseHover = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                new Color(Color.LightYellow, 0.75f),
                0.099f
            );

            Lightning = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                new Color(Color.White, 1f),
                0.299f
            );
     
            LineAttackNorthSouth = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '|',
                0.298f,
                _lineAttackColour
            );
            
            LineAttackEastWest = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '-',
                0.297f,
                _lineAttackColour
            );
            
            LineAttackNorthEastSouthWest = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '/',
                0.296f,
                _lineAttackColour
            );
            
            LineAttackNorthWestSouthEast = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapFont,
                '\\',
                0.295f,
                _lineAttackColour
            );          
        }
    }
}
