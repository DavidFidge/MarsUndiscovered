using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class AreaWallsDoors
{
    public Area Area { get; set; }
    public List<Point> Walls { get; set; }
    public List<Point> Doors { get; set; }
}