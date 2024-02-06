using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class PrefabInstance
{
    public Prefab Prefab { get; set; }
    public Point Location { get; set; }

    public Point GetRandomConnectorPoint(IEnhancedRandom rng)
    {
        var point = Prefab.GetRandomConnectorPoint(rng);
        
        return point + Location;
    }
}