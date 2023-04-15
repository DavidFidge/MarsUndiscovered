using GoRogue.Random;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using ShaiRandom.Generators;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Game.Components.WaveFunction;

public class WaveFunctionCollapse
{
    private List<Tile> _tiles = new();
    private int _mapWidth;
    private int _mapHeight;
    public TileResult[] CurrentState { get; private set; } 

    public WaveFunctionCollapse()
    {
    }

    public void CreateTiles(ContentManager contentManager)
    {
        var assetsList = contentManager.Load<string[]>("Content");
        var tileAttributes = contentManager.Load<TileAttributes>("WaveFunctionCollapse/MiningFacility/TileAttributes.json",
            new JsonContentLoader());

        var textures = assetsList
            .Where(a => a.StartsWith("WaveFunctionCollapse/MiningFacility") && !a.EndsWith(".json"))
            .ToDictionary(
                a => a.Split("/").Last().Replace(".png", ""),
                a => contentManager.Load<Texture2D>(a));

        CreateTiles(textures, tileAttributes);
    }

    public void CreateTiles(Dictionary<string, Texture2D> textures, TileAttributes tileAttributes)
    {
        foreach (var texture in textures)
        {
            var tile = new TileContent
            {
                Name = texture.Key,
                Texture = texture.Value,
                Attributes = tileAttributes.Tiles[texture.Key]
            };

            _tiles.AddRange(tile.ProcessTiles());
        }
    }

    public void Initialise(int mapWidth, int mapHeight)
    {
        _mapHeight = mapHeight;
        _mapWidth = mapWidth;

        CurrentState = new TileResult[mapWidth * mapHeight];

        for (var y = 0; y < mapHeight; y++)
        {
            for (var x = 0; x < mapWidth; x++)
            {
                var point = new Point(x, y);

                CurrentState[point.ToIndex(mapWidth)] = new TileResult(point, mapWidth);
            }
        }
    }

    public bool NextStep()
    {
        var entropy = CurrentState
            .OrderBy(t => t.Entropy)
            .ToList();

        entropy = entropy.TakeWhile(e => e.Entropy == entropy.First().Entropy).ToList();

        var chosenTile = entropy[GlobalRandom.DefaultRNG.RandomIndex(entropy)];

        var validTiles = _tiles.ToList();

        foreach (var neighbour in chosenTile.Neighbours)
        {
            if (!neighbour.IsCollapsed)
            {
                neighbour.Entropy--;
            }
            else
            {
                validTiles = validTiles.Where(t => t.CanAdaptTo(chosenTile, neighbour)).ToList();
            }
        }

        if (!validTiles.Any())
            return false;

        chosenTile.SetTile(validTiles[GlobalRandom.DefaultRNG.RandomIndex(validTiles)]);

        return true;
    }
}