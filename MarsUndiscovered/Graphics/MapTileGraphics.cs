using FrigidRogue.MonoGame.Core.Graphics.Quads;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MarsUndiscovered.Graphics;

public class MapTileGraphics
{
    private readonly MapTileGraphics _fallbackTiles;
    public SpriteSheet SpriteSheet { get; private set; }
    
    private Dictionary<string, List<MapTileTexture>> _rawMapTileTextures = new();
    private Dictionary<string, IMapTileTexture> _mapTileTextures = new();
    private Dictionary<string, MapTileTexture> _staticMapTileTextures = new();
    private List<SpriteSheetMapTileTexture> _spriteSheetMapTileTextures = new();

    public MapTileGraphics()
    {
    }

    public MapTileGraphics(MapTileGraphics fallbackTiles)
    {
        _fallbackTiles = fallbackTiles;
    }

    public void AddMapTileTextures(TileGraphicType tileGraphicType, params MapTileTexture[] mapTileTextureFrames)
    {
        _rawMapTileTextures.Add(Enum.GetName(tileGraphicType)!, mapTileTextureFrames.ToList());
    }

    public void AddMapTileTextures(Breed breed, params MapTileTexture[] mapTileTextureFrames)
    {
        _rawMapTileTextures.Add(GetMonsterKey(breed), mapTileTextureFrames.ToList());
    }

    public void AddMapTileTextures(TileGraphicFeatureType tileGraphicFeatureType, char c, params MapTileTexture[] mapTileTextureFrames)
    {
        _rawMapTileTextures.Add(GetFeatureKey(tileGraphicFeatureType, c), mapTileTextureFrames.ToList());
    }

    private static string GetMonsterKey(Breed breed)
    {
        return $"{Enum.GetName(TileGraphicType.Monster)}{breed.Name}";
    }

    private static string GetFeatureKey(TileGraphicFeatureType tileGraphicFeatureType, char c)
    {
        return $"{Enum.GetName(TileGraphicType.Feature)!}{Enum.GetName(tileGraphicFeatureType)!}{c}";
    }

    public void Build(GraphicsDevice graphicsDevice)
    {
        var atlas = CreateAtlas(graphicsDevice);

        SpriteSheet = new SpriteSheet
        {
            TextureAtlas = atlas
        };

        var atlasIndex = 0;

        foreach (var key in _rawMapTileTextures.Keys)
        {
            var spriteSheetAnimationCycle = new SpriteSheetAnimationCycle();
            spriteSheetAnimationCycle.IsLooping = true;
            spriteSheetAnimationCycle.FrameDuration = Constants.MapTileAnimationTime;

            for (var frameAtlasIndex = atlasIndex; frameAtlasIndex < atlasIndex + _rawMapTileTextures[key].Count; frameAtlasIndex++)
            {
                var spriteSheetAnimationFrame = new SpriteSheetAnimationFrame(frameAtlasIndex, Constants.MapTileAnimationTime);
                spriteSheetAnimationCycle.Frames.Add(spriteSheetAnimationFrame);
            }

            atlasIndex += _rawMapTileTextures[key].Count;

            SpriteSheet.Cycles.Add(key, spriteSheetAnimationCycle);

            var spriteSheetAnimation = SpriteSheet.CreateAnimation(key);
            spriteSheetAnimation.Play();

            var spriteSheetMapTileTexture =
                new SpriteSheetMapTileTexture(spriteSheetAnimation, _rawMapTileTextures[key].First().Opacity);

            _spriteSheetMapTileTextures.Add(spriteSheetMapTileTexture);
            _mapTileTextures.Add(key, spriteSheetMapTileTexture);
            _staticMapTileTextures.Add(key, _rawMapTileTextures[key].First());
        }
    }

    public void Update(IGameTimeService gameTimeService)
    {
        foreach (var animation in _spriteSheetMapTileTextures)
        {
            animation.Update(gameTimeService);
        }
    }

    public IMapTileTexture GetMapTileTexture(TileGraphicType tileGraphicType)
    {
        var tileKey = Enum.GetName(tileGraphicType)!;
        return _mapTileTextures.ContainsKey(tileKey)
            ? _mapTileTextures[tileKey]
            : _fallbackTiles.GetMapTileTexture(tileGraphicType);
    }

    public IMapTileTexture GetMapTileTexture(Breed breed)
    {
        var tileKey = GetMonsterKey(breed);
        return _mapTileTextures.ContainsKey(tileKey)
            ? _mapTileTextures[tileKey]
            : _fallbackTiles.GetMapTileTexture(breed);
    }

    public IMapTileTexture GetMapTileTexture(ItemType itemType)
    {
        var tileKey = GetItemTypeKey(itemType);
        return _mapTileTextures.ContainsKey(tileKey)
            ? _mapTileTextures[tileKey]
            : _fallbackTiles.GetMapTileTexture(itemType);
    }

    public IMapTileTexture GetMapTileTexture(TileGraphicFeatureType tileGraphicAnimationType, char c)
    {
        var s = GetFeatureKey(tileGraphicAnimationType, c);
        return _mapTileTextures.ContainsKey(s)
            ? _mapTileTextures[s]
            : _fallbackTiles.GetMapTileTexture(tileGraphicAnimationType, c);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(TileGraphicType tileGraphicType)
    {
        var s = Enum.GetName(tileGraphicType)!;
        return _staticMapTileTextures.ContainsKey(s)
            ? _staticMapTileTextures[s].Texture2D
            : _fallbackTiles.GetStaticTexture(tileGraphicType);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(Breed breed)
    {
        var s = GetMonsterKey(breed);
        return _staticMapTileTextures.ContainsKey(s)
            ? _staticMapTileTextures[s].Texture2D
            : _fallbackTiles.GetStaticTexture(breed);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(ItemType itemType)
    {
        var s = GetItemTypeKey(itemType);
        return _staticMapTileTextures.ContainsKey(s)
            ? _staticMapTileTextures[s].Texture2D
            : _fallbackTiles.GetStaticTexture(itemType);
    }

    public string GetItemTypeKey(ItemType itemType)
    {
        var itemTypeKey = itemType switch
        {
            Weapon _ => Enum.GetName(TileGraphicType.Weapon),
            Gadget _ => Enum.GetName(TileGraphicType.Gadget),
            NanoFlask _ => Enum.GetName(TileGraphicType.NanoFlask),
            ShipRepairParts _ =>Enum.GetName(TileGraphicType.ShipRepairParts),
            _ => throw new Exception($"Unknown item type {itemType}")
        };

        return itemTypeKey;
    }

    private TextureAtlas CreateAtlas(GraphicsDevice graphicsDevice)
    {
        var allTiles = _rawMapTileTextures.SelectMany(m => m.Value).ToList();

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

        for (var y = 0; y < perfectSquare; y++)
        {
            for (var x = 0; x < perfectSquare; x++)
            {
                var nextTile = allTiles[tileIndex++];

                nextTile.Texture2D.GetData(textureData);

                var rectangle = new Rectangle(x * Constants.TileWidth, y * Constants.TileHeight, Constants.TileWidth,
                    Constants.TileHeight);

                texture.SetData(0, rectangle, textureData, 0, textureData.Length);

                if (tileIndex >= allTiles.Count)
                    break;
            }

            if (tileIndex >= allTiles.Count)
                break;
        }

        var atlas = TextureAtlas.Create("Tiles", texture, Constants.TileWidth, Constants.TileHeight);

        return atlas;
    }
}