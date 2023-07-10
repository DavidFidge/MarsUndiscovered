using GoRogue.GameFramework;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;

public class EmptyFloorRule : MarsMapPointChoiceRule
{
    private Map _map;
    
    public override bool IsValid(Point point)
    {
        if (_map.GetTerrainAt<Floor>(point) != null && _map.GetObjectsAt(point).Count() == 1)
            return true;
        
        return false;
    }

    public override void AssignMap(MarsMap map)
    {
        _map = map;
    }
}