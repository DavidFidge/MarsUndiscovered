using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.GameFramework;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public abstract class BaseGameObjectGenerator : BaseComponent
    {
        protected Point GetPosition<T>(T spawnMonsterParams, Map map) where T : BaseSpawnGameObject
        {
            if (spawnMonsterParams.Position != null)
                return spawnMonsterParams.Position.Value;

            if (spawnMonsterParams.AvoidPosition != null)
                return map.RandomPositionAwayFrom(
                    spawnMonsterParams.AvoidPosition.Value,
                    spawnMonsterParams.AvoidPositionRange,
                    MapHelpers.EmptyPointOnFloor
                );

            return map.RandomPosition(MapHelpers.EmptyPointOnFloor);
        }
    }
}
