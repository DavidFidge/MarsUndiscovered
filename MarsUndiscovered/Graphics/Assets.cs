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
using MonoGame.Extended.TextureAtlases;

namespace MarsUndiscovered.Graphics;

public class MapTileGraphics
{
    public SpriteSheet SpriteSheet { get; private set; }
    
    private Dictionary<string, List<MapTileTexture>> _mapTileTextures = new();

    public void AddMapTileTextures(TileAnimationType tileAnimationType, params MapTileTexture[] mapTileTextureFrames)
    {
        _mapTileTextures.Add(Enum.GetName(tileAnimationType)!, mapTileTextureFrames.ToList());
    }

    public void AddMapTileTextures(Breed breed, params MapTileTexture[] mapTileTextureFrames)
    {
        _mapTileTextures.Add($"{Enum.GetName(TileAnimationType.Monster)}{breed.Name}", mapTileTextureFrames.ToList());
    }

    public void Build(GraphicsDevice graphicsDevice)
    {
        var atlas = CreateAtlas(graphicsDevice);

        SpriteSheet = new SpriteSheet
        {
            TextureAtlas = atlas
        };

        var atlasIndex = 0;

        foreach (var key in _mapTileTextures.Keys)
        {
            var spriteSheetAnimationCycle = new SpriteSheetAnimationCycle();
            spriteSheetAnimationCycle.IsLooping = true;

            for (var frameAtlasIndex = atlasIndex; frameAtlasIndex < atlasIndex + _mapTileTextures[key].Count; frameAtlasIndex++)
            {
                var spriteSheetAnimationFrame = new SpriteSheetAnimationFrame(frameAtlasIndex, Constants.MapTileAnimationTime);
                spriteSheetAnimationCycle.Frames.Add(spriteSheetAnimationFrame);
            }

            atlasIndex += _mapTileTextures[key].Count;

            SpriteSheet.Cycles.Add(key, spriteSheetAnimationCycle);
        }
    }

    private TextureAtlas CreateAtlas(GraphicsDevice graphicsDevice)
    {
        var allTiles = _mapTileTextures.SelectMany(m => m.Value).ToList();

        var squareRoot = Math.Sqrt(allTiles.Count);

        var perfectSquare = (int)squareRoot;

        if (Math.Abs(squareRoot - perfectSquare) > double.Epsilon)
        {
            perfectSquare++;
        }

        var texture = new Texture2D(graphicsDevice, Constants.TileWidth * perfectSquare,
            Constants.TileHeight * perfectSquare);

        var tileIndex = 0;
        var textureData = new Color[Constants.TileWidth * Constants.TileHeight];

        for (var x = 0; x < perfectSquare; x++)
        {
            for (var y = 0; y < perfectSquare; y++)
            {
                var nextTile = allTiles[tileIndex++];

                nextTile.Texture2D.GetData(textureData);

                var rectangle = new Rectangle(x * Constants.TileWidth, y * Constants.TileHeight, Constants.TileWidth,
                    Constants.TileHeight);

                texture.SetData(0, rectangle, textureData, 0, textureData.Length);

                if (tileIndex >= allTiles.Count)
                    break;
            }
        }

        var atlas = TextureAtlas.Create("Tiles", texture, Constants.TileWidth, Constants.TileHeight);

        return atlas;
    }
}

public enum TileAnimationType
{
    Wall,
    Floor,
    Player,
    MapExitDown,
    MapExitUp,
    Weapon,
    Gadget,
    NanoFlask,
    LineAttackEastWest,
    LineAttackNorthEastSouthWest,
    LineAttackNorthWestSouthEast,
    LineAttackNorthSouth,
    MouseHover,
    Lightning,
    ShipRepairParts,
    FieldOfViewUnrevealedTexture,
    FieldOfViewHasBeenSeenTexture,
    Monster
}

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
    private MapTileGraphics _mapTileGraphics;

    public Assets(IGameProvider gameProvider)
    {
        _gameProvider = gameProvider;
    }

    public void LoadContent()
    {
        _mapTileGraphics = new MapTileGraphics();
        Monsters = new Dictionary<string, MapTileTexture>();
        TitleTexture = _gameProvider.Game.Content.Load<Texture2D>("images/Title");
        TitleTextTexture = _gameProvider.Game.Content.Load<Texture2D>("images/TitleText");
        UiRegularFont = _gameProvider.Game.Content.Load<SpriteFont>("GeonBit.UI/themes/mars/fonts/Regular");
        MapBitmapFont = _gameProvider.Game.Content.Load<Texture2D>("fonts/BitmapFont");
        GoalMapFont = _gameProvider.Game.Content.Load<SpriteFont>("fonts/GoalMapFont");
        ShipAiRadioComms = _gameProvider.Game.Content.Load<SpriteSheet>("animations/ShipAiRadioComms.sf", new JsonContentLoader());

        Wall = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '#',
            0.999f,
            Color.White,
            WallColor
        );
        
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.Wall, Wall);
        
        Floor = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0xfa,
            0.998f,
            Color.Tan
        );
        
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.Floor, Floor);
        
        MapExitDown = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
                
            '>',
            0.997f,
            _itemColour,
            Color.SaddleBrown
        );
        
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.MapExitDown, MapExitDown);

        MapExitUp = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '<',
            0.996f,
            Color.SaddleBrown
        );
            
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.MapExitUp, MapExitUp);

        ShipParts = new Dictionary<char, MapTileTexture>();
        var shipPartChars = "{_-`.+|( ";

        foreach (char ch in shipPartChars)
        {
            var shipPartDrawDepth = ch * 0.000001f;
            var shipPart = new MapTileTexture(
                _gameProvider,
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
                _gameProvider,
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
            
        Weapon = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x18,
            0.598f,
            _itemColour
        );
        
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.Weapon, Gadget);

        Gadget = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)237,
            0.597f,
            _itemColour
        );
        
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.Gadget, Gadget);

        NanoFlask = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x9a,
            0.596f,
            _itemColour
        );

        _mapTileGraphics.AddMapTileTextures(TileAnimationType.NanoFlask, NanoFlask);

        ShipRepairParts = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '&',
            0.595f,
            _itemColour
        );

        _mapTileGraphics.AddMapTileTextures(TileAnimationType.ShipRepairParts, ShipRepairParts);

        var actorDrawDepth = 0.4999f;
            
        Player = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '@',
            actorDrawDepth,
            Color.Yellow
        );
        
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.Player, Player);

        actorDrawDepth -= 0.0001f;

        foreach (var breed in Breed.Breeds)
        {
            var monster = new MapTileTexture(
                _gameProvider,
                Constants.TileWidth,
                Constants.TileHeight,
                MapBitmapFont,
                breed.Value.AsciiCharacter,
                actorDrawDepth,
                breed.Value.ForegroundColour,
                breed.Value.BackgroundColour
            );
            
            _mapTileGraphics.AddMapTileTextures(breed.Value, monster);

            Monsters.Add(breed.Key, monster);
        }
        
        FieldOfViewUnrevealedTexture = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black,
            0.199f
        );

        _mapTileGraphics.AddMapTileTextures(TileAnimationType.FieldOfViewUnrevealedTexture, FieldOfViewUnrevealedTexture);

        FieldOfViewHasBeenSeenTexture = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black.WithTransparency(0.8f),
            0.198f
        );

        _mapTileGraphics.AddMapTileTextures(TileAnimationType.FieldOfViewHasBeenSeenTexture, FieldOfViewHasBeenSeenTexture);

        MouseHover = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.LightYellow, 0.75f),
            0.099f
        );

        _mapTileGraphics.AddMapTileTextures(TileAnimationType.MouseHover, MouseHover);

        Lightning = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.White, 1f),
            0.299f
        );

        _mapTileGraphics.AddMapTileTextures(TileAnimationType.Lightning, Lightning);

        LineAttackNorthSouth = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '|',
            0.298f,
            _lineAttackColour
        );
        
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.LineAttackNorthSouth, LineAttackNorthSouth);
            
        LineAttackEastWest = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '-',
            0.297f,
            _lineAttackColour
        );
        
        _mapTileGraphics.AddMapTileTextures(TileAnimationType.LineAttackEastWest, LineAttackEastWest);
            
        LineAttackNorthEastSouthWest = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '/',
            0.296f,
            _lineAttackColour
        );

        _mapTileGraphics.AddMapTileTextures(TileAnimationType.LineAttackNorthEastSouthWest, LineAttackNorthEastSouthWest);

        LineAttackNorthWestSouthEast = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '\\',
            0.295f,
            _lineAttackColour
        );

        _mapTileGraphics.AddMapTileTextures(TileAnimationType.LineAttackNorthWestSouthEast, LineAttackNorthWestSouthEast);

        GoalMapTileTexture = new GoalMapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            GoalMapFont,
            Color.White
        );
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
    
    public MapTileTexture GetTextureForItemType(ItemType itemType)
    {
        MapTileTexture mapTileTexture = itemType switch
        {
            Weapon _ => Weapon,
            Gadget _ => Gadget,
            NanoFlask _ => NanoFlask,
            ShipRepairParts _ => ShipRepairParts,
            _ => throw new Exception($"Unknown item type {itemType}")
        };

        return mapTileTexture;
    }
}