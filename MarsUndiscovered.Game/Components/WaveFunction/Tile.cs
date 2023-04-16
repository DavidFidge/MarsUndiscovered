using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.WaveFunction;

public class Tile
{
    private TileContent _tileContent;
    public double Weight => _tileContent.Attributes.Weight;
    public Texture2D Texture => _tileContent.Texture;

    public SpriteEffects SpriteEffects;
    public float Rotation; 
    public Dictionary<Direction, Adapter> Adapters { get; set; }

    public Tile(TileContent tileContent, Dictionary<Direction, Adapter> adapters, SpriteEffects spriteEffects = SpriteEffects.None, float rotation = 0f)
    {
        _tileContent = tileContent;
        Adapters = adapters;
        SpriteEffects = spriteEffects;
        Rotation = rotation;
    }

    public bool CanAdaptTo(TileResult firstTile, TileResult secondTile)
    {
        var direction = Direction.GetCardinalDirection(firstTile.Point, secondTile.Point);

        var firstAdapter = firstTile.Tile.Adapters[direction];
        var secondAdapter = secondTile.Tile.Adapters[direction.Opposite()];

        return Equals(firstAdapter.Pattern, secondAdapter.Pattern.Reverse());
    }
}