using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.GameFramework;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;

public class NoGameObjectTypeRule<T> : MarsMapPointChoiceRule where T : MarsGameObject
{
    private Map _map;
    
    public override bool IsValid(Point point)
    {
        return _map.GetObjectsAt<T>(point).IsEmpty();
    }

    public override void AssignMap(MarsMap map)
    {
        _map = map;
    }
}