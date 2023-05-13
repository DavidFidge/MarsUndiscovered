using FrigidRogue.MonoGame.Core.Graphics.Quads;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Game.Components;
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
    private double _accumulatedUpdateTime;
    private bool _useAnimations;

    public MapTileGraphics()
    {
    }

    public MapTileGraphics(MapTileGraphics fallbackTiles)
    {
        _fallbackTiles = fallbackTiles;
    }

    public void AddMapTileTextures(string key, params MapTileTexture[] mapTileTextureFrames)
    {
        _rawMapTileTextures.Add(key, mapTileTextureFrames.ToList());
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

            var spriteSheetMapTileTexture =
                new SpriteSheetMapTileTexture(spriteSheetAnimation, _rawMapTileTextures[key].First().Opacity);

            _spriteSheetMapTileTextures.Add(spriteSheetMapTileTexture);
            _mapTileTextures.Add(key, spriteSheetMapTileTexture);
            _staticMapTileTextures.Add(key, _rawMapTileTextures[key].First());
        }
    }

    public void UseAnimations(bool useAnimations)
    {
        _useAnimations = useAnimations;

        foreach (var animation in _spriteSheetMapTileTextures)
        {
            if (!useAnimations)
                animation.Stop();
            else
                animation.Play();
        }
    }

    public void Update(IGameTimeService gameTimeService)
    {
        if (!_useAnimations)
            return;

        _accumulatedUpdateTime += gameTimeService.GameTime.ElapsedGameTime.TotalSeconds;

        if (_accumulatedUpdateTime > Constants.MapTileAnimationTime)
        {
            foreach (var animation in _spriteSheetMapTileTextures)
            {
                animation.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(_accumulatedUpdateTime)));
            }

            _accumulatedUpdateTime = 0;
        }
    }

    public IMapTileTexture GetMapTileTexture(TileGraphicType tileGraphicType)
    {
        return GetMapTileTexture(tileGraphicType.ToString());
    }

    public IMapTileTexture GetMapTileTexture(string tileKey)
    {
        return _mapTileTextures.ContainsKey(tileKey)
            ? _mapTileTextures[tileKey]
            : _fallbackTiles.GetMapTileTexture(tileKey);
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(TileGraphicType tileGraphicType)
    {
        return GetStaticTexture(tileGraphicType.ToString());
    }

    // For use with ad-hoc requests to get an individual tile texture.
    // Do not use for drawing a map as it does not come from a texture atlas.
    // Current use case is for drawing an image in the inventory. Could consider changing
    // it to use texture atlases and animations later and removing this method.
    public Texture2D GetStaticTexture(string tileKey)
    {
        return _staticMapTileTextures.ContainsKey(tileKey)
            ? _staticMapTileTextures[tileKey].Texture2D
            : _fallbackTiles.GetStaticTexture(tileKey);
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