using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.GameFramework;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public abstract class BaseGameObjectGenerator : BaseComponent
    {
        protected Point GetPosition<T>(T spawnGameObjectParams, Map map) where T : BaseSpawnGameObjectParams
        {
            if (spawnGameObjectParams.Position != null)
                return spawnGameObjectParams.Position.Value;

            if (spawnGameObjectParams.AvoidPosition != null)
                return map.RandomPositionAwayFrom(
                    spawnGameObjectParams.AvoidPosition.Value,
                    spawnGameObjectParams.AvoidPositionRange,
                    MapHelpers.EmptyPointOnFloor
                );

            return map.RandomPosition(MapHelpers.EmptyPointOnFloor);
        }
    }
}
