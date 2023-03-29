using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MediatR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;

namespace MarsUndiscovered.Graphics;

public class Assets : IAssets, IRequestHandler<UseAsciiTilesRequest>
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
    private readonly IGameOptionsStore _gameOptionsStore;

    private Color _itemColour = Color.Yellow;
    private Color _lineAttackColour = Color.LightGray;
    
    private MapTileGraphics _asciiMapTileGraphics;
    private MapTileGraphics _graphicalMapTileGraphics;
    private MapTileGraphics _currentMapTileGraphics;

    public Assets(
        IGameProvider gameProvider,
        IGameOptionsStore gameOptionsStore)
    {
        _gameProvider = gameProvider;
        _gameOptionsStore = gameOptionsStore;
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

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Wall, wall);
        AddGraphicalMapTileTexture(assetsList, TileGraphicType.Wall,
            $"{tilesPrefix}/{Enum.GetName(TileGraphicType.Wall)}");

        var floor = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0xfa,
            Color.Tan
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Floor, floor);
        AddGraphicalMapTileTexture(assetsList, TileGraphicType.Floor,
            $"{tilesPrefix}/{Enum.GetName(TileGraphicType.Floor)}");

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
        AddGraphicalMapTileTexture(assetsList, TileGraphicType.Weapon,
            $"{tilesPrefix}/{Enum.GetName(TileGraphicType.Weapon)}");

        var gadget = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)237,
            _itemColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Gadget, gadget);
        AddGraphicalMapTileTexture(assetsList, TileGraphicType.Gadget,
            $"{tilesPrefix}/{Enum.GetName(TileGraphicType.Gadget)}");

        var nanoFlask = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x9a,
            _itemColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.NanoFlask, nanoFlask);
        AddGraphicalMapTileTexture(assetsList, TileGraphicType.NanoFlask,
            $"{tilesPrefix}/{Enum.GetName(TileGraphicType.NanoFlask)}");

        var shipRepairParts = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '&',
            _itemColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.ShipRepairParts, shipRepairParts);
        AddGraphicalMapTileTexture(assetsList, TileGraphicType.ShipRepairParts,
            $"{tilesPrefix}/{Enum.GetName(TileGraphicType.ShipRepairParts)}");

        var player = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '@',
            Color.Yellow
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.Player, player);
        AddGraphicalMapTileTexture(assetsList, TileGraphicType.Player,
            $"{tilesPrefix}/{Enum.GetName(TileGraphicType.Player)}");

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
            AddGraphicalMapTileTexture(assetsList, breed.Value, $"{tilesPrefix}/{breed.Key}");
        }

        var fieldOfViewUnrevealedTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.FieldOfViewUnrevealedTexture,
            fieldOfViewUnrevealedTexture);

        var fieldOfViewHasBeenSeenTexture = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black.WithTransparency(0.8f)
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.FieldOfViewHasBeenSeenTexture,
            fieldOfViewHasBeenSeenTexture);

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

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthEastSouthWest,
            lineAttackNorthEastSouthWest);

        var lineAttackNorthWestSouthEast = new MapTileTexture(
            _gameProvider.Game.GraphicsDevice,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '\\',
            _lineAttackColour
        );

        _asciiMapTileGraphics.AddMapTileTextures(TileGraphicType.LineAttackNorthWestSouthEast,
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

        SetTileGraphics(gameOptionsData.State.UseAsciiTiles);
    }

    private void SetTileGraphics(bool useAsciiTiles)
    {
        _currentMapTileGraphics = useAsciiTiles
            ? _asciiMapTileGraphics
            : _graphicalMapTileGraphics;
    }

    private void AddGraphicalMapTileTexture(string[] assetsList, TileGraphicType tileGraphicType, string tileAssetPattern)
    {
        _graphicalMapTileGraphics.AddMapTileTextures(
            tileGraphicType,
            GetTileAssets(assetsList, tileAssetPattern)
        );
    }

    private void AddGraphicalMapTileTexture(string[] assetsList, Breed breed, string tileAssetPattern)
    {
        _graphicalMapTileGraphics.AddMapTileTextures(
            breed,
            GetTileAssets(assetsList, tileAssetPattern)
        );
    }

    private MapTileTexture[] GetTileAssets(string[] assetsList, string pattern)
    {
        return assetsList
            .Where(asset =>
                Regex.Match(asset, pattern, RegexOptions.IgnoreCase)
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
        return _currentMapTileGraphics.GetMapTileTexture(tileGraphicType);
    }

    public IMapTileTexture GetMapTileTexture(Breed breed)
    {
        return _currentMapTileGraphics.GetMapTileTexture(breed);
    }

    public IMapTileTexture GetMapTileTexture(ItemType itemType)
    {
        return _currentMapTileGraphics.GetMapTileTexture(itemType);
    }

    public IMapTileTexture GetMapTileTexture(TileGraphicFeatureType tileGraphicFeatureType, char c)
    {
        return _currentMapTileGraphics.GetMapTileTexture(tileGraphicFeatureType, c);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(TileGraphicType tileGraphicType)
    {
        return _currentMapTileGraphics.GetStaticTexture(tileGraphicType);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(Breed breed)
    {
        return _currentMapTileGraphics.GetStaticTexture(breed);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(ItemType itemType)
    {
        return _currentMapTileGraphics.GetStaticTexture(itemType);
    }

    public Task<Unit> Handle(UseAsciiTilesRequest request, CancellationToken cancellationToken)
    {
        SetTileGraphics(request.UseAsciiTiles);
        return Unit.Task;
    }
}