using GoRogue.GameFramework;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;

public class WallAdjacentToFloorRule : MarsMapPointChoiceRule
{
    private Map _map;
    
    public override bool IsValid(Point point)
    {
        if (_map.GetTerrainAt<Wall>(point) == null || _map.GetObjectsAt(point).Count() > 1)
            return false;
        
        if (AdjacencyRule.EightWay.Neighbors(point).Any(n => _map.Contains(n) && _map.GetTerrainAt<Floor>(n) != null))
            return true;

        return false;
    }

    public override void AssignMap(MarsMap map)
    {
        _map = map;
    }
}