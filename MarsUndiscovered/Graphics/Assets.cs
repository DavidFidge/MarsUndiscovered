using System.Text.RegularExpressions;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.View.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.UserInterface.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;

namespace MarsUndiscovered.Graphics;

public class Assets : IAssets
{
    // Alpha comes first, then reverse the hex if copying from Paint.NET
    // In Breeds.csv there is no alpha and the hex can by copied directly
    public static Color WallColor = new Color(0xFF244BB6);
    public static Color WallMiningFacilityColor = new Color(0xFF777777);
    public static Color UserInterfaceColor = new Color(0xFF1E0097);

    public SpriteFont UiRegularFont { get; set; }
    public SpriteSheet ShipAiRadioComms { get; set; }
    public Texture2D TitleTexture { get; set; }
    public Texture2D TitleTextTexture { get; set; }
    public Texture2D MapBitmapFont { get; set; }
    public SpriteFont GoalMapFont { get; set; }
    public GoalMapTileTexture GoalMapTileTexture { get; set; }

    private readonly IGameProvider _gameProvider;
    private readonly IGameOptionsStore _gameOptionsStore;
    private readonly IGameTimeService _gameTimeService;

    private Color _lineAttackColour = Color.LightGray;
    
    private MapTileGraphics _asciiMapTileGraphics;
    private MapTileGraphics _graphicalMapTileGraphics;
    private MapTileGraphics _currentMapTileGraphics;

    public Assets(
        IGameProvider gameProvider,
        IGameOptionsStore gameOptionsStore,
        IGameTimeService gameTimeService)
    {
        _gameProvider = gameProvider;
        _gameOptionsStore = gameOptionsStore;
        _gameTimeService = gameTimeService;
    }

    public void LoadContent()
    {
        _asciiMapTileGraphics = new MapTileGraphics();
        _graphicalMapTileGraphics = new MapTileGraphics(_asciiMapTileGraphics);

        TitleTexture = _gameProvider.Game.Content.Load<Texture2D>("images/Title");
        TitleTextTexture = _gameProvider.Game.Content.Load<Texture2D>("images/TitleText");
        UiRegularFont = _gameProvider.Game.Content.Load<SpriteFont>("GeonBit.UI/themes/mars/fonts/Regular");
        MapBitmapFont = _gameProvider.Game.Content.Load<Texture2D>("fonts/BitmapFont");
        GoalMapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/GoalMapFont");
        ShipAiRadioComms =
            _gameProvider.Game.Content.Load<SpriteSheet>("animations/ShipAiRadioComms.sf", new JsonContentLoader());
        var assetsList = _gameProvider.Game.Content.Load<string[]>("Content");
        var tilesPrefix = "tiles";

        var wall = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '#',
            Color.White,
            WallColor
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Wall.ToString(), wall);
        _graphicalMapTileGraphics.AddMapTileTextures(
            TileGraphicType.Wall.ToString(),
            GetTileAssets(assetsList, $"{tilesPrefix}/{TileGraphicType.Wall.ToString()}")
        );
        
        var floor = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0xfa,
            Color.Tan
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Floor.ToString(), floor);
        _graphicalMapTileGraphics.AddMapTileTextures(
            TileGraphicType.Floor.ToString(),
            GetTileAssets(assetsList, $"{tilesPrefix}/{TileGraphicType.Floor.ToString()}")
        );

        var mapExitDown = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '>',
            Color.Yellow,
            Color.SaddleBrown
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.MapExitDown.ToString(), mapExitDown);

        var mapExitUp = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '<',
            Color.Yellow,
            Color.SaddleBrown
        );
        
        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.MapExitUp.ToString(), mapExitUp);

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

            _asciiMapTileGraphics.AddMapTileTextures($"{TileGraphicType.Ship}{ch}", shipPart);
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

            _asciiMapTileGraphics.AddMapTileTextures($"{TileGraphicType.MiningFacility}{ch}", miningFacilitySection);
        }
        
        var itemTypes = ItemType.ItemTypes

        var itemTypes = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x18,
            _itemColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Weapon.ToString(), weapon);
        _graphicalMapTileGraphics.AddMapTileTextures(
            TileGraphicType.Weapon.ToString(),
            GetTileAssets(assetsList, $"{tilesPrefix}/{TileGraphicType.Weapon.ToString()}")
        );
        
        var player = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '@',
            Color.Yellow
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Player.ToString(), player);
        
        _graphicalMapTileGraphics.AddMapTileTextures(
            TileGraphicType.Player.ToString(),
            GetTileAssets(assetsList, $"{tilesPrefix}/{TileGraphicType.Player.ToString()}")
        );

        var playerDead = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '@',
            Color.Gray
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.PlayerDead.ToString(), playerDead);
        _graphicalMapTileGraphics.AddMapTileTextures(
            TileGraphicType.PlayerDead.ToString(),
            GetTileAssets(assetsList, $"{tilesPrefix}/{TileGraphicType.PlayerDead.ToString()}")
        );
        
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

            _asciiMapTileGraphics.AddMapTileTextures(breed.Key, monster);
            
            _graphicalMapTileGraphics.AddMapTileTextures(
                breed.Key,
                GetTileAssets(assetsList, $"{tilesPrefix}/{breed.Key}")
            );
        }

        var fieldOfViewUnrevealedTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.FieldOfViewUnrevealedTexture.ToString(),
            fieldOfViewUnrevealedTexture);

        var fieldOfViewHasBeenSeenTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black.WithTransparency(0.8f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.FieldOfViewHasBeenSeenTexture.ToString(),
            fieldOfViewHasBeenSeenTexture);

        var mouseHover = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.LightYellow, 0.75f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.MouseHover.ToString(), mouseHover);

        var lightning = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.White, 1f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Lightning.ToString(), lightning);

        var lineAttackNorthSouth = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '|',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthSouth.ToString(), lineAttackNorthSouth);

        var lineAttackEastWest = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '-',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackEastWest.ToString(), lineAttackEastWest);

        var lineAttackNorthEastSouthWest = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '/',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthEastSouthWest.ToString(),
            lineAttackNorthEastSouthWest);

        var lineAttackNorthWestSouthEast = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '\\',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthWestSouthEast.ToString(),
            lineAttackNorthWestSouthEast);

        GoalMapTileTexture = new GoalMapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            GoalMapFont,
            Color.White
        );

        _asciiMapTileGraphics.Build(_gameProvider.Game.GraphicsDevice);
        _graphicalMapTileGraphics.Build(_gameProvider.Game.GraphicsDevice);

        var gameOptionsData = _gameOptionsStore.GetFromStore<GameOptionsData>();

        SetTileGraphicOptions(new TileGraphicOptions(gameOptionsData.State));
    }

    public void SetTileGraphicOptions(TileGraphicOptions tileGraphicOptions)
    {
        _asciiMapTileGraphics.UseAnimations(tileGraphicOptions.UseAnimations);
        _graphicalMapTileGraphics.UseAnimations(tileGraphicOptions.UseAnimations);

        _currentMapTileGraphics = tileGraphicOptions.UseAsciiTiles
            ? _asciiMapTileGraphics
            : _graphicalMapTileGraphics;
    }

    private MapTileTexture[] GetTileAssets(string[] assetsList, string pattern)
    {
        return assetsList
            .Where(asset =>
                Regex.Match(asset, $"{pattern}_", RegexOptions.IgnoreCase)
                    .Success)
            .Select(asset =>
                new MapTileTexture(
                    _gameProvider.Game.GraphicsDevice,
                    Constants.TileWidth,
                    Constants.TileHeight,
                    _gameProvider.Game.Content.Load<Texture2D>(asset)
                )
            )
            .ToArray();
    }

    public SpriteSheet GetRadioCommsSpriteSheet(RadioCommsTypes radioCommsType)
    {
        return radioCommsType switch
        {
            RadioCommsTypes.StartGame1 => ShipAiRadioComms,
            RadioCommsTypes.StartGame2 => ShipAiRadioComms,
            RadioCommsTypes.PickupShipParts => ShipAiRadioComms,
            _ => null
        };
    }

    public IMapTileTexture GetMapTileTexture(string key)
    {
        return _currentMapTileGraphics.GetMapTileTexture(key);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(string key)
    {
        return _currentMapTileGraphics.GetStaticTexture(key);
    }

    public void Update()
    {
        _currentMapTileGraphics.Update(_gameTimeService);
    }
}