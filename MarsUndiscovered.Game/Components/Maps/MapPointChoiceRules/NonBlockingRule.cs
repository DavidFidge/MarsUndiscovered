using GoRogue.GameFramework;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;

public class NonBlockingRule : MarsMapPointChoiceRule
{
    private Map _map;
    private MarsMap _marsMap => (MarsMap)_map;
    
    public override bool IsValid(Point point)
    {
        return !_marsMap.IsBlockingIfPlacingObstacle(point);
    }

    public override void AssignMap(MarsMap map)
    {
        _map = map;
    }
}