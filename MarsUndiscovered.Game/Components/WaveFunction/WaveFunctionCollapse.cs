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
    private readonly List<Tile> _tiles = new();
    private int _mapWidth;
    private int _mapHeight;
    public TileResult[] CurrentState { get; private set; }
    public List<Tile> Tiles => _tiles;

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

            _tiles.AddRange(tile.CreateTiles());
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

        foreach (var tileResult in CurrentState)
        {
            tileResult.SetNeighbours(CurrentState, _mapWidth, _mapHeight);
        }
    }

    public NextStepResult NextStep()
    {
        var entropy = CurrentState
            .Where(t => !t.IsCollapsed)
            .OrderBy(t => t.Entropy)
            .ToList();

        if (!entropy.Any())
            return NextStepResult.Complete(); 

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
            return NextStepResult.Failed();

        chosenTile.SetTile(validTiles[GlobalRandom.DefaultRNG.RandomIndex(validTiles)]);

        return NextStepResult.Continue();
    }
}

public class NextStepResult
{
    public bool IsComplete { get; set; }
    public bool IsFailed { get; set; }
    
    public static NextStepResult Complete()
    {
        return new NextStepResult {IsComplete = true};
    }
    
    public static NextStepResult Failed()
    {
        return new NextStepResult {IsFailed = true};
    }
    
    public static NextStepResult Continue()
    {
        return new NextStepResult();
    }
}