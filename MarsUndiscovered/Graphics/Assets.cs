using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;

namespace MarsUndiscovered.Graphics;

public class Assets : IAssets
{
    // Alpha comes first, then reverse the hex if copying from Paint.NET
    public static Color WallColor = new Color(0xFF244BB6);
    public static Color UserInterfaceColor = new Color(0xFF1E0097);

    public SpriteFont UiRegularFont { get; set; }
    public SpriteSheet ShipAiRadioComms { get; set; }
    public Texture2D TitleTexture { get; set; }
    public Texture2D TitleTextTexture { get; set; }
    public Texture2D MapBitmapFont { get; set; }
    public SpriteFont GoalMapFont { get; set; }
    public GoalMapTileTexture GoalMapTileTexture { get; set; }
    public IDictionary<char, MapTileTexture> ShipParts { get; set; }
    public IDictionary<char, MapTileTexture> MiningFacilitySection { get; set; }

    private readonly IGameProvider _gameProvider;

    private Color _itemColour = Color.Yellow;
    private Color _lineAttackColour = Color.LightGray;
    
    private MapTileGraphics _asciiMapTileGraphics;

    public Assets(IGameProvider gameProvider)
    {
        _gameProvider = gameProvider;
    }

    public void LoadContent()
    {
        _asciiMapTileGraphics = new MapTileGraphics();
        TitleTexture = _gameProvider.Game.Content.Load<Texture2D>("images/Title");
        TitleTextTexture = _gameProvider.Game.Content.Load<Texture2D>("images/TitleText");
        UiRegularFont = _gameProvider.Game.Content.Load<SpriteFont>("GeonBit.UI/themes/mars/fonts/Regular");
        MapBitmapFont = _gameProvider.Game.Content.Load<Texture2D>("fonts/BitmapFont");
        GoalMapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/GoalMapFont");
        ShipAiRadioComms = _gameProvider.Game.Content.Load<SpriteSheet>("animations/ShipAiRadioComms.sf", new JsonContentLoader());

        var wall = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '#',
            0.999f,
            Color.White,
            WallColor
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.Wall, wall);
        
        var floor = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0xfa,
            0.998f,
            Color.Tan
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.Floor, floor);
        
        var mapExitDown = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '>',
            0.997f,
            _itemColour,
            Color.SaddleBrown
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.MapExitDown, mapExitDown);

        var mapExitUp = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '<',
            0.996f,
            _itemColour,
            Color.SaddleBrown
        );
            
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.MapExitUp, mapExitUp);

        ShipParts = new Dictionary<char, MapTileTexture>();
        var shipPartChars = "{_-`.+|( ";

        foreach (char ch in shipPartChars)
        {
            var shipPartDrawDepth = ch * 0.000001f;
            var shipPart = new MapTileTexture(
                _gameProvider.Game.GraphicsDevice,
                Constants.TileWidth,
                Constants.TileHeight,
                MapBitmapFont,
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
                _gameProvider.Game.GraphicsDevice,
                Constants.TileWidth,
                Constants.TileHeight,
                MapBitmapFont,
                ch,
                0.8f + miningFacilitySectionDrawDepth,
                Color.MediumPurple,
                Color.Black
            );

            MiningFacilitySection.Add(ch, miningFacilitySection);
        }
            
        var weapon = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x18,
            0.598f,
            _itemColour
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.Weapon, weapon);

        var gadget = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)237,
            0.597f,
            _itemColour
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.Gadget, gadget);

        var nanoFlask = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x9a,
            0.596f,
            _itemColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.NanoFlask, nanoFlask);

        var shipRepairParts = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '&',
            0.595f,
            _itemColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.ShipRepairParts, shipRepairParts);

        var actorDrawDepth = 0.4999f;
            
        var player = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '@',
            actorDrawDepth,
            Color.Yellow
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.Player, player);

        actorDrawDepth -= 0.0001f;

        foreach (var breed in Breed.Breeds)
        {
            var monster = new MapTileTexture(
                _gameProvider.Game.GraphicsDevice,
                Constants.TileWidth,
                Constants.TileHeight,
                MapBitmapFont,
                breed.Value.AsciiCharacter,
                actorDrawDepth,
                breed.Value.ForegroundColour,
                breed.Value.BackgroundColour
            );

            _asciiMapTileGraphics.AddMapTileTextures(breed.Value, monster);
        }
        
        var fieldOfViewUnrevealedTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black,
            0.199f
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.FieldOfViewUnrevealedTexture, fieldOfViewUnrevealedTexture);

        var fieldOfViewHasBeenSeenTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black.WithTransparency(0.8f),
            0.198f
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.FieldOfViewHasBeenSeenTexture, fieldOfViewHasBeenSeenTexture);

        var mouseHover = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.LightYellow, 0.75f),
            0.099f
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.MouseHover, mouseHover);

        var lightning = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.White, 1f),
            0.299f
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.Lightning, lightning);

        var lineAttackNorthSouth = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '|',
            0.298f,
            _lineAttackColour
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.LineAttackNorthSouth, lineAttackNorthSouth);
            
        var lineAttackEastWest = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '-',
            0.297f,
            _lineAttackColour
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.LineAttackEastWest, lineAttackEastWest);
            
        var lineAttackNorthEastSouthWest = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '/',
            0.296f,
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.LineAttackNorthEastSouthWest, lineAttackNorthEastSouthWest);

        var lineAttackNorthWestSouthEast = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '\\',
            0.295f,
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileAnimationType.LineAttackNorthWestSouthEast, lineAttackNorthWestSouthEast);

        GoalMapTileTexture = new GoalMapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            GoalMapFont,
            Color.White
        );

        _asciiMapTileGraphics.Build(_gameProvider.Game.GraphicsDevice);
    }

    public SpriteSheet GetRadioCommsSpriteSheet(IGameObject gameObject)
    {
        return gameObject switch
        {
            Ship => ShipAiRadioComms,
            Item { ItemType: Components.ShipRepairParts } => ShipAiRadioComms,
            _ => null
        };
    }

    public IMapTileTexture GetMapTileTexture(TileAnimationType tileAnimationType)
    {
        return _asciiMapTileGraphics.GetMapTileTexture(tileAnimationType);
    }

    public IMapTileTexture GetMapTileTexture(Breed breed)
    {
        return _asciiMapTileGraphics.GetMapTileTexture(breed);
    }

    public IMapTileTexture GetMapTileTexture(ItemType itemType)
    {
        return _asciiMapTileGraphics.GetMapTileTexture(itemType);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(TileAnimationType tileAnimationType)
    {
        return _asciiMapTileGraphics.GetStaticTexture(tileAnimationType);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(Breed breed)
    {
        return _asciiMapTileGraphics.GetStaticTexture(breed);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(ItemType itemType)
    {
        return _asciiMapTileGraphics.GetStaticTexture(itemType);
    }
}