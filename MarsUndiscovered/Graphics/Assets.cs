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
    public TextureAtlas AsciiAtlas { get; set; }
    public TextureAtlas TileAtlas { get; set; }
    
    public AnimatedSprite AsciiSprites { get; set; }

    public MapTileGraphics()
    {
        var spriteSheet = new SpriteSheet();
        spriteSheet.TextureAtlas = AsciiAtlas;

        var animation = spriteSheet.CreateAnimation("test");
        animation.KeyFrames = 
        AsciiSprites = new AnimatedSprite(AsciiAtlas, )
        
        // current idea - go with having all specific types and specific methods e.g. AddWallFrame, AddFloorFrame,
        // AddBreedFrame etc holding each in a dictionary.
        // After this, create a list of all the frames.  Get the next perfect square, create a texture of that size,
        // then write all the frames to it.  You can then call TextureAtlas.Create which will automatically
        // create all the regions and you can index into it with the indexer.
        // Then create SpriteSheetAnimationCycle objects and also creating the frames, the frame Index property must be
        // set to the texture atlas index.  We will now have AnimatedSprites for each of Wall, Floor, monster breeds,
        // ship parts etc.
        
        // We can have one of these for the Ascii graphics and another one for the Tile graphics. The graphics
        // can then easily be changed if the game settings change.
    }
    
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

    public Assets(IGameProvider gameProvider)
    {
        _gameProvider = gameProvider;
    }

    public void LoadContent()
    {
        var allTextures = new List<MapTileTexture>();
        
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
        
        allTextures.Add(Wall);

        Floor = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0xfa,
            0.998f,
            Color.Tan
        );
        
        allTextures.Add(Floor);
        
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
        
        allTextures.Add(MapExitDown);

        MapExitUp = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '<',
            0.996f,
            Color.SaddleBrown
        );
            
        allTextures.Add(MapExitUp);

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
            
            allTextures.Add(shipPart);
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
            
            allTextures.Add(miningFacilitySection);
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
        
        allTextures.Add(Weapon);

        Gadget = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)237,
            0.597f,
            _itemColour
        );

        allTextures.Add(Gadget);

        NanoFlask = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            (char)0x9a,
            0.596f,
            _itemColour
        );
        
        allTextures.Add(NanoFlask);

        ShipRepairParts = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '&',
            0.595f,
            _itemColour
        );
        
        allTextures.Add(ShipRepairParts);
            
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

        allTextures.Add(Player);

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
            
            allTextures.Add(monster);

            Monsters.Add(breed.Key, monster);
        }
        
        FieldOfViewUnrevealedTexture = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black,
            0.199f
        );

        allTextures.Add(FieldOfViewUnrevealedTexture);

        FieldOfViewHasBeenSeenTexture = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            Color.Black.WithTransparency(0.8f),
            0.198f
        );
        
        allTextures.Add(FieldOfViewHasBeenSeenTexture);

        MouseHover = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.LightYellow, 0.75f),
            0.099f
        );
        
        allTextures.Add(MouseHover);

        Lightning = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            new Color(Color.White, 1f),
            0.299f
        );
        
        allTextures.Add(Lightning);
     
        LineAttackNorthSouth = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '|',
            0.298f,
            _lineAttackColour
        );
        
        allTextures.Add(LineAttackNorthSouth);
            
        LineAttackEastWest = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '-',
            0.297f,
            _lineAttackColour
        );
        
        allTextures.Add(LineAttackEastWest);
            
        LineAttackNorthEastSouthWest = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '/',
            0.296f,
            _lineAttackColour
        );
        
        allTextures.Add(LineAttackNorthEastSouthWest);
            
        LineAttackNorthWestSouthEast = new MapTileTexture(
            _gameProvider,
            Constants.TileWidth,
            Constants.TileHeight,
            MapBitmapFont,
            '\\',
            0.295f,
            _lineAttackColour
        );
        
        allTextures.Add(LineAttackNorthWestSouthEast);

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