using FrigidRogue.MonoGame.Core.Extensions;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.WaveFunction;

public class TileResult
{
    public List<TileResult> Neighbours { get; private set; }
    public Point Point { get; }
    public int Index { get; }
    public bool IsCollapsed => Tile != null;
    public Tile Tile { get; set; }
    public int Entropy { get; set; } = Int32.MaxValue;

    public TileResult(Point point, int maxWidth)
    {
        Point = point;
        Index = point.ToIndex(maxWidth);
    }

    public void SetNeighbours(TileResult[] tiles, int maxWidth, int maxHeight)
    {
        Neighbours = Point
            .Neighbours(maxWidth, maxHeight)
            .Select(n => tiles[n.ToIndex(maxWidth)])
            .ToList();
    }

    public void SetTile(Tile tile)
    {
        Tile = tile;
    }
}