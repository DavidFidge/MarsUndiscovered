using FrigidRogue.MonoGame.Core.Graphics.Quads;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MarsUndiscovered.Graphics;

public class MapTileGraphics
{
    private readonly MapTileGraphics _asciiTiles;
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

    public MapTileGraphics(MapTileGraphics asciiTiles)
    {
        _asciiTiles = asciiTiles;
    }

    public void AddMapTileTextures(string key, params MapTileTexture[] mapTileTextureFrames)
    {
        if (mapTileTextureFrames.Any())
            _rawMapTileTextures.Add(key, mapTileTextureFrames.ToList());
    }

    public void Build(GraphicsDevice graphicsDevice)
    {
        var atlas = CreateAtlas(graphicsDevice);

        SpriteSheet = new SpriteSheet("main", atlas);

        var atlasIndex = 0;

        foreach (var key in _rawMapTileTextures.Keys)
        {
            SpriteSheet.DefineAnimation(key, builder =>
            {
                builder.IsLooping(true);

                for (var frameAtlasIndex = atlasIndex;
                     frameAtlasIndex < atlasIndex + _rawMapTileTextures[key].Count;
                     frameAtlasIndex++)
                {
                    builder.AddFrame(frameAtlasIndex, TimeSpan.FromSeconds(UiConstants.MapTileAnimationSeconds));
                }
            });

            atlasIndex += _rawMapTileTextures[key].Count;

            var spriteSheetMapTileTexture =
                new SpriteSheetMapTileTexture(new AnimatedSprite(SpriteSheet, key), _rawMapTileTextures[key].First().Opacity);

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

        if (_asciiTiles != null)
            _asciiTiles.UseAnimations(useAnimations);
    }

    public void Update(IGameTimeService gameTimeService)
    {
        if (!_useAnimations)
            return;

        _accumulatedUpdateTime += gameTimeService.GameTime.ElapsedGameTime.TotalSeconds;

        if (_accumulatedUpdateTime > UiConstants.MapTileAnimationSeconds)
        {
            foreach (var animation in _spriteSheetMapTileTextures)
            {
                animation.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(_accumulatedUpdateTime)));
            }

            _accumulatedUpdateTime = 0;
        }

        if (_asciiTiles != null)
            _asciiTiles.Update(gameTimeService);
    }

    public IMapTileTexture GetMapTileTexture(string tileKey)
    {
        return _mapTileTextures.ContainsKey(tileKey)
            ? _mapTileTextures[tileKey]
            : _asciiTiles.GetMapTileTexture(tileKey);
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
            : _asciiTiles.GetStaticTexture(tileKey);
    }

    private Texture2DAtlas CreateAtlas(GraphicsDevice graphicsDevice)
    {
        var allTiles = _rawMapTileTextures.SelectMany(m => m.Value).ToList();

        var squareRoot = Math.Sqrt(allTiles.Count);

        var perfectSquare = (int)squareRoot;

        if (Math.Abs(squareRoot - perfectSquare) > double.Epsilon)
        {
            perfectSquare++;
        }

        var texture = new Texture2D(graphicsDevice, UiConstants.TileWidth * perfectSquare,
            UiConstants.TileHeight * perfectSquare);

        var tileIndex = 0;
        var textureData = new Color[UiConstants.TileWidth * UiConstants.TileHeight];

        for (var y = 0; y < perfectSquare; y++)
        {
            for (var x = 0; x < perfectSquare; x++)
            {
                var nextTile = allTiles[tileIndex++];

                nextTile.Texture2D.GetData(textureData);

                var rectangle = new Rectangle(x * UiConstants.TileWidth, y * UiConstants.TileHeight, UiConstants.TileWidth,
                    UiConstants.TileHeight);

                texture.SetData(0, rectangle, textureData, 0, textureData.Length);

                if (tileIndex >= allTiles.Count)
                    break;
            }

            if (tileIndex >= allTiles.Count)
                break;
        }

        var atlas = Texture2DAtlas.Create("Tiles", texture, UiConstants.TileWidth, UiConstants.TileHeight);

        return atlas;
    }
}