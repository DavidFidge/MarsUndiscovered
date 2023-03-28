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
            Color.White,
            WallColor
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Wall, wall);
        
        var floor = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0xfa,
            Color.Tan
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Floor, floor);
        
        var mapExitDown = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '>',
            _itemColour,
            Color.SaddleBrown
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.MapExitDown, mapExitDown);

        var mapExitUp = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '<',
            _itemColour,
            Color.SaddleBrown
        );
            
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.MapExitUp, mapExitUp);

        var shipPartChars = "{_-`.+|( ";

        foreach (char ch in shipPartChars)
        {
            var shipPart = new MapTileTexture(
                _gameProvider.Game.GraphicsDevice,
                Constants.TileWidth,
                Constants.TileHeight,
                MapBitmapFont,
                ch,
                Color.SteelBlue,
                Color.Black
            );

            _asciiMapTileGraphics.AddMapTileTextures(TileGraphicFeatureType.Ship, ch, shipPart);
        }

        var miningFacilitySectionChars = @"_()|#-`'.+:\/= ";

        foreach (char ch in miningFacilitySectionChars)
        {
            var miningFacilitySection = new MapTileTexture(
                _gameProvider.Game.GraphicsDevice,
                Constants.TileWidth,
                Constants.TileHeight,
                MapBitmapFont,
                ch,
                Color.MediumPurple,
                Color.Black
            );

            _asciiMapTileGraphics.AddMapTileTextures(TileGraphicFeatureType.MiningFacility, ch, miningFacilitySection);
        }

        var weapon = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x18,
            _itemColour
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Weapon, weapon);

        var gadget = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)237,
            _itemColour
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Gadget, gadget);

        var nanoFlask = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x9a,
            _itemColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.NanoFlask, nanoFlask);

        var shipRepairParts = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '&',
            _itemColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.ShipRepairParts, shipRepairParts);

        var player = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '@',
            Color.Yellow
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Player, player);

        foreach (var breed in Breed.Breeds)
        {
            var monster = new MapTileTexture(
                _gameProvider.Game.GraphicsDevice,
                Constants.TileWidth,
                Constants.TileHeight,
                MapBitmapFont,
                breed.Value.AsciiCharacter,
                breed.Value.ForegroundColour,
                breed.Value.BackgroundColour
            );

            _asciiMapTileGraphics.AddMapTileTextures(breed.Value, monster);
        }
        
        var fieldOfViewUnrevealedTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.FieldOfViewUnrevealedTexture, fieldOfViewUnrevealedTexture);

        var fieldOfViewHasBeenSeenTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black.WithTransparency(0.8f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.FieldOfViewHasBeenSeenTexture, fieldOfViewHasBeenSeenTexture);

        var mouseHover = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.LightYellow, 0.75f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.MouseHover, mouseHover);

        var lightning = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.White, 1f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Lightning, lightning);

        var lineAttackNorthSouth = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '|',
            _lineAttackColour
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthSouth, lineAttackNorthSouth);
            
        var lineAttackEastWest = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '-',
            _lineAttackColour
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackEastWest, lineAttackEastWest);
            
        var lineAttackNorthEastSouthWest = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '/',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthEastSouthWest, lineAttackNorthEastSouthWest);

        var lineAttackNorthWestSouthEast = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '\\',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthWestSouthEast, lineAttackNorthWestSouthEast);

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

    public IMapTileTexture GetMapTileTexture(TileGraphicType tileGraphicType)
    {
        return _asciiMapTileGraphics.GetMapTileTexture(tileGraphicType);
    }

    public IMapTileTexture GetMapTileTexture(Breed breed)
    {
        return _asciiMapTileGraphics.GetMapTileTexture(breed);
    }

    public IMapTileTexture GetMapTileTexture(ItemType itemType)
    {
        return _asciiMapTileGraphics.GetMapTileTexture(itemType);
    }

    public IMapTileTexture GetMapTileTexture(TileGraphicFeatureType tileGraphicFeatureType, char c)
    {
        return _asciiMapTileGraphics.GetMapTileTexture(tileGraphicFeatureType, c);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(TileGraphicType tileGraphicType)
    {
        return _asciiMapTileGraphics.GetStaticTexture(tileGraphicType);
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