using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.WaveFunction;

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