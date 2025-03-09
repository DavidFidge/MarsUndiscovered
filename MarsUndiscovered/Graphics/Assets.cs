using System.Text.RegularExpressions;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.View.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Serialization.Json;

namespace MarsUndiscovered.Graphics;

public class Assets : IAssets
{
    // Alpha comes first, then reverse the hex if copying from Paint.NET
    // In Breeds.csv there is no alpha and the hex can by copied directly
    public static Color UserInterfaceColor = new Color(0xFF1E0097);
    private const string _tilesPrefix = "tiles";
    private const string _iconsPrefix = "icons";

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

        var shipAiRadioCommsTexture = _gameProvider.Game.Content.Load<Texture2D>("animations/ShipAiRadioComms");
        var shipAiRadioCommsTextureAtlas = Texture2DAtlas.Create("ShipAiRadioComms", shipAiRadioCommsTexture, 32, 32);
        ShipAiRadioComms = new SpriteSheet("ShipAiRadioComms", shipAiRadioCommsTextureAtlas);
        
        ShipAiRadioComms.DefineAnimation("talk", b =>
        {
            b.IsLooping(true);
            b.AddFrame(0, TimeSpan.FromSeconds(1));
            b.AddFrame(1, TimeSpan.FromSeconds(1));
        });
        
        var assetsList = _gameProvider.Game.Content.Load<string[]>("Content");

        foreach (var wallType in WallType.WallTypes)
        {
            CreateGameObjectTypeGraphics(wallType.Value, assetsList, wallType.Key);
        }

        foreach (var floorType in FloorType.FloorTypes)
        {
            CreateGameObjectTypeGraphics(floorType.Value, assetsList, floorType.Key);
        }
        
        foreach (var doorType in DoorType.DoorTypes)
        {
            CreateGameObjectTypeGraphics(doorType.Value, assetsList, doorType.Key);
        }
        
        foreach (var featureType in FeatureType.FeatureTypes)
        {
            CreateGameObjectTypeGraphics(featureType.Value, assetsList, featureType.Key);
        }
        
        foreach (var machineType in MachineType.MachineTypes)
        {
            CreateGameObjectTypeGraphics(machineType.Value, assetsList, machineType.Key);
        }
        
        var itemTypes = ItemType.ItemTypes.Values
            .GroupBy(i => i.AsciiCharacter)
            .Select(g => g.First()).ToList();

        foreach (var itemType in itemTypes)
        {
            CreateGameObjectTypeGraphics(itemType, assetsList, itemType.GetAbstractTypeName());
        }
        
        foreach (var mapExitType in MapExitType.MapExitTypes)
        {
            CreateGameObjectTypeGraphics(mapExitType.Value, assetsList, mapExitType.Key);
        }
        
        var shipPartChars = ShipGenerator.ShipPrefab.PrefabText
            .SelectMany(s => s)
            .Distinct()
            .ToArray();

        foreach (char ch in shipPartChars)
        {
            var shipPart = new MapTileTexture(
                _gameProvider.Game.GraphicsDevice,
                UiConstants.TileWidth,
                UiConstants.TileHeight,
                MapBitmapFont,
                ch,
                Color.SteelBlue,
                Color.Black
            );

            _asciiMapTileGraphics.AddMapTileTextures($"{TileGraphicType.Ship}{ch}", shipPart);
        }
        
        var player = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            MapBitmapFont,
            '@',
            Color.Yellow
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Player.ToString(), player);
        
        _graphicalMapTileGraphics.AddMapTileTextures(
            TileGraphicType.Player.ToString(),
            GetTileAssets(assetsList, $"{_tilesPrefix}/{TileGraphicType.Player.ToString()}")
        );

        var playerDead = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            MapBitmapFont,
            '@',
            Color.Gray
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.PlayerDead.ToString(), playerDead);
        _graphicalMapTileGraphics.AddMapTileTextures(
            TileGraphicType.PlayerDead.ToString(),
            GetTileAssets(assetsList, $"{_tilesPrefix}/{TileGraphicType.PlayerDead.ToString()}")
        );
        
        foreach (var breed in Breed.Breeds)
        {
            var monster = new MapTileTexture(
                _gameProvider.Game.GraphicsDevice,
                UiConstants.TileWidth,
                UiConstants.TileHeight,
                MapBitmapFont,
                breed.Value.AsciiCharacter,
                breed.Value.ForegroundColour,
                breed.Value.BackgroundColour
            );

            _asciiMapTileGraphics.AddMapTileTextures(breed.Key, monster);
            
            _graphicalMapTileGraphics.AddMapTileTextures(
                breed.Key,
                GetTileAssets(assetsList, $"{_tilesPrefix}/{breed.Key}")
            );
        }

        var fieldOfViewUnrevealedTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            Color.Black
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.FieldOfViewUnrevealedTexture.ToString(),
            fieldOfViewUnrevealedTexture);

        var fieldOfViewHasBeenSeenTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            Color.Black.WithTransparency(0.8f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.FieldOfViewHasBeenSeenTexture.ToString(),
            fieldOfViewHasBeenSeenTexture);

        var mouseHover = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            new Color(Color.LightYellow, 0.75f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.MouseHover.ToString(), mouseHover);

        var lightning = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            new Color(Color.White, 1f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Lightning.ToString(), lightning);

        var laser = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            new Color(Color.Red, 1f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Laser.ToString(), laser);
        
        var lineAttackNorthSouth = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            MapBitmapFont,
            '|',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthSouth.ToString(), lineAttackNorthSouth);

        var lineAttackEastWest = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            MapBitmapFont,
            '-',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackEastWest.ToString(), lineAttackEastWest);

        var lineAttackNorthEastSouthWest = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            MapBitmapFont,
            '/',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthEastSouthWest.ToString(),
            lineAttackNorthEastSouthWest);

        var lineAttackNorthWestSouthEast = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            MapBitmapFont,
            '\\',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthWestSouthEast.ToString(),
            lineAttackNorthWestSouthEast);

        GoalMapTileTexture = new GoalMapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            GoalMapFont,
            Color.White
        );
        
        CreateWeaknessIcons(assetsList);

        _asciiMapTileGraphics.Build(_gameProvider.Game.GraphicsDevice);
        _graphicalMapTileGraphics.Build(_gameProvider.Game.GraphicsDevice);

        var gameOptionsData = _gameOptionsStore.GetFromStore<GameOptionsData>();

        SetTileGraphicOptions(new TileGraphicOptions(gameOptionsData.State));
    }

    private void CreateWeaknessIcons(string[] assetsList)
    {
        foreach (var weakness in Enum.GetNames(typeof(WeaknessesEnum)))
        {
            _graphicalMapTileGraphics.AddMapTileTextures(
                $"{weakness}Active",
                GetTileAssets(assetsList, $"{_iconsPrefix}/{weakness}Active")
            );
            
            _graphicalMapTileGraphics.AddMapTileTextures(
                $"{weakness}Inactive",
                GetTileAssets(assetsList, $"{_iconsPrefix}/{weakness}Inactive")
            );
        }
    }
    
    private void CreateGameObjectTypeGraphics<T>(T gameObjectType, string[] assetsList, string assetKey) where T : GameObjectType
    {
        var mapTileTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            UiConstants.TileWidth,
            UiConstants.TileHeight,
            MapBitmapFont,
            gameObjectType.AsciiCharacter,
            gameObjectType.ForegroundColour,
            gameObjectType.BackgroundColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(assetKey, mapTileTexture);
        _graphicalMapTileGraphics.AddMapTileTextures(
            assetKey,
            GetTileAssets(assetsList, $"{_tilesPrefix}/{assetKey}")
        );
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
        var tiles = assetsList
            .Where(asset =>
                Regex.Match(asset, $"{pattern}_", RegexOptions.IgnoreCase)
                    .Success)
            .Select(asset =>
                new MapTileTexture(
                    _gameProvider.Game.GraphicsDevice,
                    UiConstants.TileWidth,
                    UiConstants.TileHeight,
                    _gameProvider.Game.Content.Load<Texture2D>(asset)
                )
            )
            .ToArray();
        
        if (tiles.Any())
            return tiles;

        return assetsList
            .Where(asset =>
                Regex.Match(asset, $"{pattern}", RegexOptions.IgnoreCase)
                    .Success)
            .Select(asset =>
                new MapTileTexture(
                    _gameProvider.Game.GraphicsDevice,
                    UiConstants.TileWidth,
                    UiConstants.TileHeight,
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