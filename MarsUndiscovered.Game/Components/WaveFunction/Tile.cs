using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.WaveFunction;

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

    public bool CanAdaptTo(TileResult firstTile, TileResult secondTile)
    {
        var direction = Direction.GetCardinalDirection(firstTile.Point, secondTile.Point);

        var firstAdapter = firstTile.Tile.Adapters[direction];
        var secondAdapter = secondTile.Tile.Adapters[direction.Opposite()];

        return Equals(firstAdapter.Pattern, secondAdapter.Pattern.Reverse());
    }
}