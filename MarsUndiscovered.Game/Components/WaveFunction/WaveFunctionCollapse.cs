using System.Drawing;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.Random;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using SadRogue.Primitives;
using ShaiRandom.Generators;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Game.Components.WaveFunction;

public class TileAttributes
{
    public Dictionary<string, TileAttribute> Tiles = new();
}

public class TileAttribute
{
    public string Symmetry { get; set; }
    public double Weight { get; set; }
    public string Adapters { get; set; }
}

public class TileContent
{
    public string Name { get; set; }
    public TileAttribute Attributes { get; set; }
    public Texture2D Texture { get; set; }

    public List<Tile> ProcessTiles()
    {
        var tiles = new List<Tile>();

        if (Attributes.Symmetry == "X")
        {
            var adapters = AdjacencyRule.Cardinals.DirectionsOfNeighborsCache
                .ToDictionary(x => x, x => (Adapter)Attributes.Adapters);

            tiles.Add(new Tile(this, adapters));
        }
        if (Attributes.Symmetry == "L")
        {
        }

        return tiles;
    }
}

public class Tile
{
    private TileContent _tileContent;
    public double Weight => _tileContent.Attributes.Weight;
    public Texture2D Texture => _tileContent.Texture;

    public SpriteEffects SpriteEffects = SpriteEffects.None;
    public float Rotation = 0f; 
    public Dictionary<Direction, Adapter> Adapters { get; set; } = new();

    public Tile(TileContent tileContent, Dictionary<Direction, Adapter> adapters)
    {
        _tileContent = tileContent;
        Adapters = adapters;
    }
}

public class TileResult
{
    public int Index { get; }
    public bool IsCollapsed => Tile != null;
    public Tile Tile { get; set; }
    public int Entropy { get; set; } = Int32.MaxValue;

    public TileResult(int index)
    {
        Index = index;
    }
}

public class Adapter
{
    public string Pattern { get; set; }

    public static implicit operator Adapter(string pattern)
    {
        return new Adapter { Pattern = pattern };
    }
}

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

        foreach (var asset in assetsList)
        {
            if (asset.StartsWith("WaveFunctionCollapse/MiningFacility") && !asset.EndsWith(".json"))
            {
                var name = asset.Split("/").Last().Replace(".png", "");

                var tile = new TileContent
                {
                    Name = name,
                    Texture = contentManager.Load<Texture2D>(asset),
                    Attributes = tileAttributes.Tiles[name],
                };

                _tiles.AddRange(tile.ProcessTiles());
            }
        }
    }

    public void Initialise(int mapWidth, int mapHeight)
    {
        _mapHeight = mapHeight;
        _mapWidth = mapWidth;

        var CurrentState = new TileResult[mapWidth * mapHeight];

        for (var y = 0; y < mapHeight; y++)
        {
            for (var x = 0; x < mapWidth; x++)
            {
                CurrentState[x + (y * mapWidth)] = new TileResult(x + (y * mapWidth));
            }
        }
    }

    public void NextStep()
    {
        var entropy = CurrentState
            .OrderBy(t => t.Entropy)
            .ToList();

        entropy = entropy.TakeWhile(e => e.Entropy == entropy.First().Entropy).ToList();

        var randomIndex = GlobalRandom.DefaultRNG.RandomIndex(entropy);

        var neighbours = Point.FromIndex(randomIndex, _mapWidth)
            .Neighbours(_mapWidth, _mapHeight)
            .Select(n => CurrentState[n.ToIndex(_mapWidth)])
            .ToList();

        foreach (var neighbour in neighbours)
        {
            if (!neighbour.IsCollapsed)
            {
                neighbour.Entropy--;
            }
            else
            {
                 
            }

        }



    }


}